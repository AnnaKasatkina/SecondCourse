public class MyTask<TResult>(MyThreadPool threadPool, Func<TResult> func) : IMyTask<TResult>
{
    private readonly MyThreadPool threadPool = threadPool ?? throw new ArgumentNullException(nameof(threadPool));
    private readonly Func<TResult> func = func ?? throw new ArgumentNullException(nameof(func));
    private readonly List<Action> continuations = new List<Action>();
    private readonly object _lock = new object();
    private bool isCompleted = false;
    private TResult? result;
    private Exception? exception;

    public bool IsCompleted
    {
        get
        {
            lock (_lock)
            {
                return isCompleted;
            }
        }
    }

    public TResult Result
    {
        get
        {
            lock (_lock)
            {
                while (!isCompleted)
                {
                    Monitor.Wait(_lock);
                }

                if (exception != null)
                {
                    throw new AggregateException(exception);
                }

                if (result == null)
                {
                    throw new InvalidOperationException("The result is null!");
                }

                return result;
            }
        }
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        if (continuation == null)
        {
            throw new ArgumentNullException(nameof(continuation));
        }

        var newTask = new MyTask<TNewResult>(threadPool, () =>
        {
            TResult parentResult = this.Result;
            return continuation(parentResult);
        });

        lock (_lock)
        {
            if (!isCompleted)
            {
                threadPool.EnqueueTask(() => newTask.Execute());
            }
            else
            {
                continuations.Add(() => threadPool.EnqueueTask(() => newTask.Execute()));
            }
        }

        return newTask;
    }

    public void Execute()
    {
        try
        {
            TResult res = func();
            List<Action> continuationsCopy;
            lock (_lock)
            {
                result = res;
                isCompleted = true;
                continuationsCopy = new List<Action>(continuations);
                continuations.Clear();
                Monitor.PulseAll(_lock);
            }

            foreach (var cont in continuationsCopy)
            {
                cont.Invoke();
            }
        }

        catch (Exception ex)
        {
            List<Action> continuationsCopy;
            lock (_lock)
            {
                exception = ex;
                isCompleted = true;
                continuationsCopy = new List<Action>(continuations);
                continuations.Clear();
                Monitor.PulseAll(_lock);
            }

            foreach (var cont in continuationsCopy)
            {
                cont.Invoke();
            }
        }
    }
}