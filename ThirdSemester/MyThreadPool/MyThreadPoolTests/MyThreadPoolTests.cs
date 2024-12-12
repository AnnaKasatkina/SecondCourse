// <copyright file="MyThreadPoolTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyThreadPoolTests;

[TestFixture]
public class MyThreadPoolTests
{
    [Test]
    public void Task_Should_Execute_And_Return_Result()
    {
        var threadPool = new MyThreadPool.MyThreadPool(3);
        var task = threadPool.Submit(() => 2 * 2);

        Assert.That(task.Result, Is.EqualTo(4));
        threadPool.Shutdown();
    }

    [Test]
    public void Task_Should_ContinueWith_And_Return_New_Result()
    {
        var threadPool = new MyThreadPool.MyThreadPool(3);

        var task = threadPool.Submit(() => 10);
        var continuedTask = task.ContinueWith(result => result.ToString());

        Assert.That(continuedTask.Result, Is.EqualTo("10"));
        threadPool.Shutdown();
    }

    [Test]
    public void Pool_Should_Process_Multiple_Tasks_Concurrently()
    {
        var threadPool = new MyThreadPool.MyThreadPool(3);

        var task1 = threadPool.Submit(() =>
        {
            Thread.Sleep(500);
            return 1;
        });
        var task2 = threadPool.Submit(() =>
        {
            Thread.Sleep(500);
            return 2;
        });
        var task3 = threadPool.Submit(() =>
        {
            Thread.Sleep(500);
            return 3;
        });
        Assert.Multiple(() =>
        {
            Assert.That(task1.Result, Is.EqualTo(1));
            Assert.That(task2.Result, Is.EqualTo(2));
            Assert.That(task3.Result, Is.EqualTo(3));
        });

        threadPool.Shutdown();
    }

    [Test]
    public void Pool_Should_Not_Accept_New_Tasks_After_Shutdown()
    {
        var threadPool = new MyThreadPool.MyThreadPool(2);

        threadPool.Shutdown();

        Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 1));
    }

    [Test]
    public void Pool_Should_Have_At_Least_N_Threads()
    {
        var threadPool = new MyThreadPool.MyThreadPool(3);
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
}
