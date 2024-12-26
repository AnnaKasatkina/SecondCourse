// <copyright file="MyThreadPoolTests.cs" company="Anna Kasatkina">
// Copyright (c) Anna Kasatkina. All rights reserved.
// </copyright>

namespace MyThreadPoolTests;

using MyThreadPool;

/// <summary>
/// Contains unit tests for the MyThreadPool class to verify its functionality, concurrency behavior,
/// and handling of edge cases like shutdown and chained tasks.
/// </summary>
[TestFixture]
public class MyThreadPoolTests
{
    /// <summary>
    /// Verifies that a task submitted to the thread pool executes correctly and returns the expected result.
    /// </summary>
    [Test]
    public void Task_Should_Execute_And_Return_Result()
    {
        var threadPool = new MyThreadPool(3);
        var task = threadPool.Submit(() => 2 * 2);

        Assert.That(task.Result, Is.EqualTo(4));
        threadPool.Shutdown();
    }

    /// <summary>
    /// Verifies that a task can chain another task using ContinueWith and return a new result.
    /// </summary>
    [Test]
    public void Task_Should_ContinueWith_And_Return_New_Result()
    {
        var threadPool = new MyThreadPool(3);

        var task = threadPool.Submit(() => 10);
        var continuedTask = task.ContinueWith(result => result.ToString());

        Assert.That(continuedTask.Result, Is.EqualTo("10"));
        threadPool.Shutdown();
    }

    /// <summary>
    /// Ensures that the thread pool processes multiple tasks concurrently by checking task start times.
    /// </summary>
    [Test]
    public void Pool_Should_Process_Multiple_Tasks_Concurrently()
    {
        var threadPool = new MyThreadPool(3);

        var startTimes = new List<DateTime>();
        var tasks = new[]
        {
            threadPool.Submit(() =>
            {
                startTimes.Add(DateTime.Now);
                Thread.Sleep(500);
                return 1;
            }),
            threadPool.Submit(() =>
            {
                startTimes.Add(DateTime.Now);
                Thread.Sleep(500);
                return 2;
            }),
            threadPool.Submit(() =>
            {
                startTimes.Add(DateTime.Now);
                Thread.Sleep(500);
                return 3;
            }),
        };

        Task.WaitAll(tasks.Select(t => Task.Run(() => t.Result)).ToArray());

        var timeDifference = startTimes.Max() - startTimes.Min();
        Assert.That(timeDifference.TotalMilliseconds, Is.LessThan(500), "Tasks did not execute concurrently.");
        threadPool.Shutdown();
    }

    /// <summary>
    /// Ensures that the thread pool does not accept new tasks after it has been shut down.
    /// </summary>
    [Test]
    public void Pool_Should_Not_Accept_New_Tasks_After_Shutdown()
    {
        var threadPool = new MyThreadPool(2);

        threadPool.Shutdown();

        Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 1));
    }

    /// <summary>
    /// Verifies that the thread pool maintains at least the specified number of active threads.
    /// </summary>
    [Test]
    public void Pool_Should_Have_At_Least_N_Threads()
    {
        var threadPool = new MyThreadPool(3);
        int activeThreads = 0;

        var task = threadPool.Submit(() =>
        {
            Interlocked.Increment(ref activeThreads);
            Thread.Sleep(100);
            return 0;
        });

        for (int i = 0; i < 3; i++)
        {
            threadPool.Submit(() =>
            {
                Interlocked.Increment(ref activeThreads);
                Thread.Sleep(100);
                return 0;
            });
        }

        var result = task.Result;

        Assert.That(activeThreads, Is.GreaterThanOrEqualTo(3));
        threadPool.Shutdown();
    }

    /// <summary>
    /// Tests concurrent behavior during simultaneous Submit and Shutdown calls.
    /// </summary>
    [Test]
    public void Pool_Should_Handle_Concurrent_Submit_And_Shutdown()
    {
        var threadPool = new MyThreadPool(3);
        var tasks = new List<Task>();

        var submitTask = Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var tasks = new List<IMyTask<int>>();
                    tasks.Add(threadPool.Submit(() =>
                    {
                        Thread.Sleep(100);
                        return 0;
                    }));
                }
                catch (InvalidOperationException)
                {
                }
            }
        });

        var shutdownTask = Task.Run(() => threadPool.Shutdown());

        Task.WaitAll(submitTask, shutdownTask);

        Assert.Multiple(() =>
        {
            Assert.That(() => tasks.All(t => t.IsCompleted || t.IsCanceled), Is.True, "Some tasks did not complete or were not canceled.");
            Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 1), "Pool should not accept tasks after shutdown.");
        });
    }

    /// <summary>
    /// Ensures correct behavior of tasks using ContinueWith after the thread pool is shut down.
    /// </summary>
    [Test]
    public void Pool_Should_Handle_Shutdown_And_ContinueWith()
    {
        var threadPool = new MyThreadPool(3);

        var initialTask = threadPool.Submit(() =>
        {
            Thread.Sleep(100);
            return 42;
        });

        var continuedTask = initialTask.ContinueWith(result => result + 1);

        threadPool.Shutdown();

        Assert.Multiple(() =>
        {
            Assert.That(initialTask.Result, Is.EqualTo(42), "Initial task did not complete as expected.");
            Assert.That(continuedTask.Result, Is.EqualTo(43), "Continued task did not complete as expected.");
        });
    }
}
