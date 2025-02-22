// <copyright file="MyNUnitTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using MyNUnit;

namespace MyNUnitTests;

/// <summary>
/// A set of tests to verify the correctness of the MyNUnit testing framework.
/// </summary>
[TestFixture]
public class MyNUnitTests
{
    private StringWriter consoleOutput;

    /// <summary>
    /// Configures console output redirection before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.consoleOutput = new StringWriter();
        Console.SetOut(this.consoleOutput);
    }

    /// <summary>
    /// Restores standard console output and releases resources after each test.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
        this.consoleOutput.Dispose();
    }

    /// <summary>
    /// Verifies the correct execution of methods with BeforeClass and AfterClass attributes.
    /// </summary>
    [NUnit.Framework.Test]
    public void TestBeforeClassAndAfterClassMethods()
    {
        var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
        Assert.That(path, Is.Not.Null, "Path should not be null");
        TestRunner.RunTests(path);

        var output = this.consoleOutput.ToString();
        StringAssert.Contains("Before all tests", output);
        StringAssert.Contains("After all tests", output);
    }

    /// <summary>
    /// Verifies the correct execution of methods with Before and After attributes before and after each test.
    /// </summary>
    [NUnit.Framework.Test]
    public void TestBeforeAndAfterMethods()
    {
        var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
        Assert.That(path, Is.Not.Null, "Path should not be null");
        TestRunner.RunTests(path);

        var output = this.consoleOutput.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(this.CountSubstringOccurrences(output, "Before each test"), Is.EqualTo(2));
            Assert.That(this.CountSubstringOccurrences(output, "After each test"), Is.EqualTo(2));
        });
    }

    /// <summary>
    /// Verifies that the framework correctly handles an expected exception.
    /// </summary>
    [NUnit.Framework.Test]
    public void TestExpectedException()
    {
        var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
        Assert.That(path, Is.Not.Null, "Path should not be null");
        TestRunner.RunTests(path);

        var output = this.consoleOutput.ToString();
        StringAssert.Contains("TestExpectedException Passed (Expected exception)", output);
    }

    /// <summary>
    /// Verifies that ignored tests are not executed and the reason is displayed.
    /// </summary>
    [NUnit.Framework.Test]
    public void TestIgnoredTest()
    {
        var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
        Assert.That(path, Is.Not.Null, "Path should not be null");
        TestRunner.RunTests(path);

        var output = this.consoleOutput.ToString();
        StringAssert.Contains("TestIgnored Ignored: Ignored test example", output);
    }

    /// Verifies that a successful test is executed correctly.
    /// </summary>
    [NUnit.Framework.Test]
    public void TestPassedTest()
    {
        var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
        Assert.That(path, Is.Not.Null, "Path should not be null");
        TestRunner.RunTests(path);

        var output = this.consoleOutput.ToString();
        StringAssert.Contains("TestPasses Passed", output);
    }

    /// <summary>
    /// Verifies that tests are executed in parallel.
    /// </summary>
    [NUnit.Framework.Test]
    public void TestParallelExecution()
    {
        var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
        Assert.That(path, Is.Not.Null, "Path should not be null");

        var start = DateTime.Now;
        TestRunner.RunTests(path);
        var end = DateTime.Now;

        var elapsed = end - start;
        Assert.That(elapsed.TotalMilliseconds, Is.LessThanOrEqualTo(2000), "Tests did not run in parallel as expected.");
    }

    /// <summary>
    /// Helper method to count the number of substring occurrences in a text.
    /// </summary>
    /// <param name="text">The text to search in.</param>
    /// <param name="substring">The substring to count.</param>
    /// <returns>The number of occurrences of the substring in the text.</returns>
    private int CountSubstringOccurrences(string text, string substring)
    {
        int count = 0, index = 0;
        while ((index = text.IndexOf(substring, index)) != -1)
        {
            count++;
            index += substring.Length;
        }

        return count;
    }
}
