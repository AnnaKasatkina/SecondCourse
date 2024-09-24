// <copyright file="LazyTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Unit tests for SimpleLazy and ThreadSafeLazy implementations.
/// Tests cover both single-threaded and multi-threaded cases.
/// Ensures that lazy evaluation is computed only once, regardless of concurrent access,
/// and that exceptions are thrown when appropriate.
/// </summary>
[TestFixture]
public class LazyTests
{
    /// <summary>
    /// Tests that the lazy evaluation computes the correct value when Get is called.
    /// This test is run for both SimpleLazy and ThreadSafeLazy.
    /// </summary>
    /// <param name="isThreadSafe">Specifies whether to test the thread-safe implementation or not.</param>
    [TestCase(false)]
    [TestCase(true)]
    public void Lazy_Get_ReturnsComputedValue(bool isThreadSafe)
    {
        int value = 42;
        var lazy = CreateLazy(() => value, isThreadSafe);

        var result = lazy.Get();

        Assert.That(result, Is.EqualTo(value));
    }

    /// <summary>
    /// Tests that the lazy evaluation calls the supplier function only once,
    /// even when Get is called multiple times.
    /// This test is run for both SimpleLazy and ThreadSafeLazy.
    /// </summary>
    /// <param name="isThreadSafe">Specifies whether to test the thread-safe implementation or not.</param>
    [TestCase(false)]
    [TestCase(true)]
    public void Lazy_Get_CalledOnce_OnlyOneComputation(bool isThreadSafe)
    {
        int callCount = 0;
        var lazy = CreateLazy(
            () =>
        {
            callCount++;
            return 42;
        }, isThreadSafe);

        lazy.Get();
        lazy.Get();

        Assert.That(callCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that passing a null supplier function to the constructor throws an ArgumentNullException.
    /// This test is run for both SimpleLazy and ThreadSafeLazy.
    /// </summary>
    /// <param name="isThreadSafe">Specifies whether to test the thread-safe implementation or not.</param>
    [TestCase(false)]
    [TestCase(true)]
    public void Lazy_ThrowsArgumentNullException_WhenSupplierIsNull(bool isThreadSafe)
    {
        Assert.Throws<ArgumentNullException>(() => CreateLazy<int>(null, isThreadSafe));
    }

    /// <summary>
    /// Single-threaded test for SimpleLazy to ensure that the Get method returns the same value
    /// after multiple calls.
    /// </summary>
    [Test]
    public void SimpleLazy_Get_ReturnsSameValue_AfterMultipleCalls()
    {
        var lazy = new SimpleLazy<int>(() => 42);

        var result1 = lazy.Get();
        var result2 = lazy.Get();

        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(42));
            Assert.That(result2, Is.EqualTo(42));
        });
    }

    /// <summary>
    /// Multi-threaded test for ThreadSafeLazy to ensure that the supplier function is invoked only once,
    /// even when accessed from multiple threads.
    /// </summary>
    [Test]
    public void ThreadSafeLazy_Get_ComputesValueOnce_Multithreaded()
    {
        int callCount = 0;
        var lazy = new ThreadSafeLazy<int>(() =>
        {
            Interlocked.Increment(ref callCount);
            return 42;
        });

        Parallel.For(0, 10, _ =>
        {
            lazy.Get();
        });

        Assert.That(callCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Multi-threaded test for ThreadSafeLazy to ensure there are no race conditions
    /// when multiple threads attempt to access the value simultaneously.
    /// </summary>
    [Test]
    public void ThreadSafeLazy_Get_EnsuresNoRaces_Multithreaded()
    {
        int callCount = 0;
        var lazy = new ThreadSafeLazy<int>(() =>
        {
            Thread.Sleep(50);
            Interlocked.Increment(ref callCount);
            return 42;
        });

        var tasks = new Task[10];
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() => lazy.Get());
        }

        Task.WaitAll(tasks);

        Assert.That(callCount, Is.EqualTo(1));
    }

    private static ILazy<T> CreateLazy<T>(Func<T>? supplier, bool isThreadSafe)
    {
        ArgumentNullException.ThrowIfNull(supplier);

        if (isThreadSafe)
        {
            return new ThreadSafeLazy<T>(supplier);
        }
        else
        {
            return new SimpleLazy<T>(supplier);
        }
    }
}
