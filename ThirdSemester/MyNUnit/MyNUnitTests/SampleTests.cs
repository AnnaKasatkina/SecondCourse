// <copyright file="SampleTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnitTests;

/// <summary>
/// Example test class demonstrating the functionality of the MyNUnit framework.
/// </summary>
public class SampleTests
{
    /// <summary>
    /// This static method is executed once before any tests in the class are run.
    /// It can be used to initialize shared resources or perform setup tasks needed by all tests.
    /// </summary>
    [MyNUnit.Attributes.BeforeClass]
    public static void BeforeAllTests() => Console.WriteLine("Before all tests");

    /// <summary>
    /// This static method is executed once after all tests in the class have been run.
    /// It can be used to clean up shared resources or perform teardown tasks.
    /// </summary>
    [MyNUnit.Attributes.AfterClass]
    public static void AfterAllTests() => Console.WriteLine("After all tests");

    /// <summary>
    /// This instance method is executed before each individual test method.
    /// It is used to set up the test environment specific to each test.
    /// </summary>
    [MyNUnit.Attributes.Before]
    public void BeforeEachTest() => Console.WriteLine("Before each test");

    /// <summary>
    /// This test method is expected to throw an <see cref="InvalidOperationException"/>.
    /// If the exception is thrown as expected, the test is considered to have passed.
    /// </summary>
    [MyNUnit.Test(Expected = typeof(InvalidOperationException))]
    public void TestExpectedException() => throw new InvalidOperationException();

    /// <summary>
    /// This test method is marked to be ignored.
    /// It will not be executed and the ignore reason is provided in the attribute.
    /// </summary>
    [MyNUnit.Test(Ignore = "Ignored test example")]
    public void TestIgnored()
    {
    }

    /// <summary>
    /// This test method is designed to pass successfully.
    /// It outputs a message indicating that the test should pass.
    /// </summary>
    [MyNUnit.Test]
    public void TestPasses() => Console.WriteLine("This test should pass.");

    /// <summary>
    /// This instance method is executed after each individual test method.
    /// It is used to clean up or reset any changes made during the test execution.
    /// </summary>
    [MyNUnit.Attributes.After]
    public void AfterEachTest() => Console.WriteLine("After each test");
}
