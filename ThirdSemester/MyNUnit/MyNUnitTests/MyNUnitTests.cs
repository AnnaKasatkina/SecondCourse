// <copyright file="MyNUnitTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit.Tests
{
    /// <summary>
    /// ����� ������ ��� �������� ������������ ������ ������� MyNUnit.
    /// </summary>
    [NUnit.Framework.TestFixture]
    public class MyNUnitTests
    {
        private StringWriter consoleOutput;

        /// <summary>
        /// ����������� �������� ������ � ������� ����� ������ ������.
        /// </summary>
        [NUnit.Framework.SetUp]
        public void SetUp()
        {
            this.consoleOutput = new StringWriter();
            Console.SetOut(this.consoleOutput);
        }

        /// <summary>
        /// ��������������� ����������� ����� � ������� � ����������� ������� ����� ���������� �����.
        /// </summary>
        [NUnit.Framework.TearDown]
        public void TearDown()
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            this.consoleOutput.Dispose();
        }

        /// <summary>
        /// ��������� ������������ ���������� ������� � ���������� BeforeClass � AfterClass.
        /// </summary>
        [NUnit.Framework.Test]
        public void TestBeforeClassAndAfterClassMethods()
        {
            var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
            Assert.IsNotNull(path, "Path should not be null");
            TestRunner.RunTests(path);

            var output = this.consoleOutput.ToString();
            StringAssert.Contains("Before all tests", output);
            StringAssert.Contains("After all tests", output);
        }

        /// <summary>
        /// ��������� ������������ ���������� ������� � ���������� Before � After ����� � ����� ������� �����.
        /// </summary>
        [NUnit.Framework.Test]
        public void TestBeforeAndAfterMethods()
        {
            var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
            Assert.IsNotNull(path, "Path should not be null");
            TestRunner.RunTests(path);

            var output = this.consoleOutput.ToString();
            Assert.That(this.CountSubstringOccurrences(output, "Before each test"), Is.EqualTo(2));
            Assert.That(this.CountSubstringOccurrences(output, "After each test"), Is.EqualTo(2));
        }

        /// <summary>
        /// ���������, ��� ���� ��������� ������������ ��������� ����������.
        /// </summary>
        [NUnit.Framework.Test]
        public void TestExpectedException()
        {
            var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
            Assert.IsNotNull(path, "Path should not be null");
            TestRunner.RunTests(path);

            var output = this.consoleOutput.ToString();
            StringAssert.Contains("TestExpectedException Passed (Expected exception)", output);
        }

        /// <summary>
        /// ���������, ��� ������������ ����� �� ����������� � ��������� �������.
        /// </summary>
        [NUnit.Framework.Test]
        public void TestIgnoredTest()
        {
            var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
            Assert.IsNotNull(path, "Path should not be null");
            TestRunner.RunTests(path);

            var output = this.consoleOutput.ToString();
            StringAssert.Contains("TestIgnored Ignored: Ignored test example", output);
        }

        /// <summary>
        /// ���������, ��� �������� ���� �������� ���������.
        /// </summary>
        [NUnit.Framework.Test]
        public void TestPassedTest()
        {
            var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
            Assert.IsNotNull(path, "Path should not be null");
            TestRunner.RunTests(path);

            var output = this.consoleOutput.ToString();
            StringAssert.Contains("TestPasses Passed", output);
        }

        /// <summary>
        /// ���������, ��� ����� ����������� �����������.
        /// </summary>
        [NUnit.Framework.Test]
        public void TestParallelExecution()
        {
            var path = Path.GetDirectoryName(typeof(SampleTests).Assembly.Location);
            Assert.IsNotNull(path, "Path should not be null");

            var start = DateTime.Now;
            TestRunner.RunTests(path);
            var end = DateTime.Now;

            var elapsed = end - start;
            Assert.LessOrEqual(elapsed.TotalMilliseconds, 2000, "Tests did not run in parallel as expected.");
        }

        /// <summary>
        /// ��������������� ����� ��� �������� ����� ��������� ���������.
        /// </summary>
        /// <param name="text">�����, � ������� ����������� �����.</param>
        /// <param name="substring">��������� ��� ������.</param>
        /// <returns>���������� ��������� ��������� � ������.</returns>
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
}
