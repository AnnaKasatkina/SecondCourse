// <copyright file="SampleTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnitTests;

/// <summary>
/// Example test class demonstrating the functionality of the MyNUnit framework.
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
    {
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
