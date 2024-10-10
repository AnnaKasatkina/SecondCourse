using System.Runtime.CompilerServices;

public class MyThreadPool : IDisposable
{
    private readonly int threadsCount;
    private readonly List<Thread> threads;
    private readonly Queue<Action> taskQueue;
    private readonly object _lock = new object();
    private bool isShutdownInitiated = false;
    private bool completePendingTasks = true;
    private readonly ManualResetEvent shurdownEvent = new ManualResetEvent(false);
    private int activeThreads;

    public MyThreadPool(int threadsCount)
    {
        if (threadsCount <= 0)
        {
            throw new ArgumentException("Количество потоков должно быть положительным.", nameof(threadsCount));
        }

        this.threadsCount = threadsCount;
        threads = new List<Thread>(threadsCount);
        taskQueue = new Queue<Action>();

        for (int i = 0; i < threadsCount; i++)
        {
            Thread thread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = $"My ThreadPool_Thread_{i}"
            };
            threads.Add(thread);
            thread.Start();
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        lock (_lock)
        {
            if (isShutdownInitiated)
            {
                throw new InvalidOperationException("ThreadPool завершает работу и не принимает новые задачи.");
            }

            var myTask = new MyTask<TResult>(this, func);
            EnqueueTask(() => myTask.Execute());
            return myTask;
        }
    }
    public void EnqueueTask(Action action)
    {
        lock (_lock)
        {
            if (isShutdownInitiated)
            {
                throw new InvalidOperationException("ThreadPool завершает работу и не принимает новые задачи.");
            }

            taskQueue.Enqueue(action);
            Monitor.Pulse(_lock);
        }
    }

    private void WorkerLoop()
    {
        while (true)
        {
            Action? task = null;
            lock (_lock)
            {
                while(taskQueue.Count == 0 && !isShutdownInitiated)
                {
                    Monitor.Wait(_lock);
                }

                if (isShutdownInitiated && (taskQueue.Count == 0 || !completePendingTasks))
                {
                    break;
                }

                if (taskQueue.Count > 0)
                {
                    task = taskQueue.Dequeue();
                }
            }

            if (task != null)
            {
                Interlocked.Increment(ref activeThreads);

                try
                {
                    task.Invoke();
                }
                finally
                {
                    Interlocked.Decrement(ref activeThreads);
                }
            }
        }

        if (Interlocked.Decrement(ref activeThreads) == 0 && taskQueue.Count == 0)
        {
            shurdownEvent.Set();
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    public void Shutdown(bool newCompletePendingTasks = true)
    {
        lock (_lock)
        {
            if (isShutdownInitiated)
            { return; }

            isShutdownInitiated = true;
            completePendingTasks = newCompletePendingTasks;

            if (!newCompletePendingTasks)
            {
                taskQueue.Clear();
            }

            Monitor.PulseAll(_lock);
        }

        shurdownEvent.WaitOne();

        foreach (var thread in threads)
        {
            if (thread.IsAlive)
            {
                thread.Join();
            }
        }
    }
}