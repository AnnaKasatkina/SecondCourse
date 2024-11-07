// <copyright file="SampleTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Пример тестового класса для демонстрации работы системы MyNUnit.
/// </summary>
public class SampleTests
{
    [MyNUnit.BeforeClass]
    public static void BeforeAllTests()
    {
        Console.WriteLine("Before all tests");
    }

    [MyNUnit.Before]
    public void BeforeEachTest()
    {
        Console.WriteLine("Before each test");
    }

    [MyNUnit.Test(Expected = typeof(InvalidOperationException))]
    public void TestExpectedException()
    {
        throw new InvalidOperationException();
    }

    [MyNUnit.Test(Ignore = "Ignored test example")]
    public void TestIgnored()
    { /* Этот тест игнорируется */
    }

    [MyNUnit.Test]
    public void TestPasses()
    {
        Console.WriteLine("This test should pass.");
    }

    [MyNUnit.After]
    public void AfterEachTest()
    {
        Console.WriteLine("After each test");
    }

    [MyNUnit.AfterClass]
    public static void AfterAllTests()
    {
        Console.WriteLine("After all tests");
    }
}