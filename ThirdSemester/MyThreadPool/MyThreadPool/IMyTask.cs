// <copyright file="IMyTask.cs" company="Anna Kasatkina">
// Copyright (c) Anna Kasatkina. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Represents an asynchronous task that can compute a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The result type of the task.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether the task has completed execution.
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the task. Blocks if the task is not completed yet.
    /// </summary>
    /// <exception cref="AggregateException">
    /// Thrown if the task encountered an exception during execution.
    /// </exception>
    TResult Result { get; }

    /// <summary>
    /// Continues the execution of the task with a new task.
    /// </summary>
    /// <typeparam name="TNewResult">The result type of the continuation task.</typeparam>
    /// <param name="continuation">
    /// Function processing the current task's result and returning a new result.
    /// </param>
    /// <returns>A new task for the continuation.</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}
