// <copyright file="MyTask.cs" company="Anna Kasatkina">
// Copyright (c) Anna Kasatkina. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Represents a task that can be executed by the thread pool.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
internal class MyTask<TResult> : IMyTask<TResult>
{
    private readonly MyThreadPool threadPool;
    private Func<TResult>? taskFunc;
    private TResult? result;
    private Exception? exception;
    private bool isCompleted;
    private ManualResetEvent completionEvent = new(false);
    private List<Action> continuations = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="taskFunc">The function representing the task to be executed.</param>
    /// <param name="threadPool">The thread pool that will execute this task.</param>
    public MyTask(Func<TResult> taskFunc, MyThreadPool threadPool)
    {
        this.taskFunc = taskFunc;
        this.threadPool = threadPool;
    }

    /// <summary>
    /// Gets a value indicating whether the task is completed.
    /// </summary>
    public bool IsCompleted => this.isCompleted;

    /// <summary>
    /// Gets the result of the task.
    /// If the task is not completed, this will block until the result is available.
    /// </summary>
    public TResult Result
    {
        get
        {
            this.completionEvent.WaitOne();
            if (this.exception != null)
            {
                throw new AggregateException(this.exception);
            }

            if (this.result == null)
            {
                throw new InvalidOperationException("The result is null!");
            }

            return this.result;
        }
    }

    /// <summary>
    /// Executes the task and stores the result.
    /// </summary>
    public void Execute()
    {
        try
        {
            if (this.taskFunc == null)
            {
                throw new InvalidOperationException("Task function is null.");
            }

            this.result = this.taskFunc();
        }
        catch (Exception ex)
        {
            this.exception = ex;
        }
        finally
        {
            this.isCompleted = true;
            this.completionEvent.Set();
            this.taskFunc = null;

            lock (this.continuations)
            {
                foreach (var continuation in this.continuations)
                {
                    this.threadPool.EnqueueTask(continuation);
                }
            }
        }
    }

    /// <summary>
    /// Schedules a continuation task that will run after this task is completed.
    /// </summary>
    /// <typeparam name="TNewResult">The type of the result produced by the continuation task.</typeparam>
    /// <param name="continuation">The function to apply to the result of the current task.</param>
    /// <returns>A new task representing the continuation.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        var newTask = new MyTask<TNewResult>(() => continuation(this.Result), this.threadPool);
        Action runContinuation = () => newTask.Execute();

        lock (this.continuations)
        {
            if (this.isCompleted)
            {
                this.threadPool.EnqueueTask(runContinuation);
            }
            else
            {
                this.continuations.Add(runContinuation);
            }
        }

        return newTask;
    }
}
