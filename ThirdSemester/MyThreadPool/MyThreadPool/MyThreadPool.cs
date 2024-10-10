// <copyright file="MyThreadPool.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Concurrent;

/// <summary>
/// Represents a thread pool that manages a fixed number of worker threads.
/// Allows submitting tasks that are executed by these threads.
/// </summary>
public class MyThreadPool
{
    private readonly int numThreads;
    private readonly Thread[] threads;
    private readonly BlockingCollection<Action> taskQueue = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class with a fixed number of threads.
    /// </summary>
    /// <param name="numThreads">The number of threads in the pool.</param>
    public MyThreadPool(int numThreads)
    {
        this.numThreads = numThreads;
        this.threads = new Thread[this.numThreads];

        for (int i = 0; i < this.numThreads; i++)
        {
            this.threads[i] = new Thread(this.WorkerLoop);
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Submits a task to the thread pool for execution.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
    /// <param name="taskFunc">The function representing the task.</param>
    /// <returns>A task object representing the submitted task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> taskFunc)
    {
        var myTask = new MyTask<TResult>(taskFunc);
        this.taskQueue.Add(() => myTask.Execute());
        return myTask;
    }

    /// <summary>
    /// Shuts down the thread pool, waiting for all tasks to complete.
    /// No new tasks can be submitted after calling this method.
    /// </summary>
    public void Shutdown()
    {
        this.cancellationTokenSource.Cancel();
        foreach (var thread in this.threads)
        {
            thread.Join();
        }

        this.taskQueue.CompleteAdding();
    }

    /// <summary>
    /// The worker loop where threads retrieve and execute tasks from the task queue.
    /// </summary>
    private void WorkerLoop()
    {
        while (!this.cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                Action task = this.taskQueue.Take(this.cancellationTokenSource.Token);
                task();
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
