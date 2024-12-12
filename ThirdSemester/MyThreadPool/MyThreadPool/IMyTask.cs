// <copyright file="IMyTask.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
    /// Gets the result of the task. If the task has not yet completed,
    /// this property blocks the calling thread until the result is available.
    /// </summary>
    /// <exception cref="AggregateException">
    /// Thrown if the task encountered an exception during execution.
    /// The exception is wrapped in an <see cref="AggregateException"/>.
    /// </exception>
    TResult Result { get; }

    /// <summary>
    /// Continues the execution of the task with a new task that will process the result of the current task.
    /// </summary>
    /// <typeparam name="TNewResult">The type of the result returned by the continuation task.</typeparam>
    /// <param name="continuation">
    /// A function that processes the result of the current task and returns a new result.
    /// </param>
    /// <returns>A new task representing the continuation.</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}