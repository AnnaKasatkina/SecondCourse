﻿// <copyright file="MyThreadPool.cs" company="Anna Kasatkina">
// Copyright (c) Anna Kasatkina. All rights reserved.
// </copyright>

namespace MyThreadPool;

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
        if (numThreads <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numThreads), "Number of threads must be positive.");
        }

        Instance = this;
        this.numThreads = numThreads;
        this.threads = new Thread[this.numThreads];

        for (int i = 0; i < this.numThreads; i++)
        {
            this.threads[i] = new Thread(this.WorkerLoop);
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <remarks>
    /// This property ensures that there is only one instance of the thread pool.
    /// Trying to create another instance will result in an exception.
    /// </remarks>
    public static MyThreadPool Instance { get; private set; } = null!;

    /// <summary>
    /// Submits a task to the thread pool for execution.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
    /// <param name="taskFunc">The function representing the task.</param>
    /// <returns>A task object representing the submitted task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> taskFunc)
    {
        if (this.taskQueue.IsAddingCompleted)
        {
            throw new InvalidOperationException("Thread pool is shutting down. Cannot accept new tasks.");
        }

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
        this.taskQueue.CompleteAdding();
        foreach (var thread in this.threads)
        {
            thread.Join();
        }

        while (this.taskQueue.TryTake(out var task))
        {
            try
            {
                task();
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Adds a task to the thread pool's queue if the pool is active.
    /// </summary>
    /// <param name="task">The task to be added to the queue.</param>
    public void EnqueueTask(Action task)
    {
        if (!this.taskQueue.IsAddingCompleted)
        {
            this.taskQueue.Add(task);
        }
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

        while (!this.taskQueue.IsCompleted)
        {
            if (this.taskQueue.TryTake(out var task))
            {
                task();
            }
        }
    }
}
