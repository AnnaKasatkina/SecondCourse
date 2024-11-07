// <copyright file="MyNUnitTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit.Tests
{
    /// <summary>
    /// Ќабор тестов дл€ проверки корректности работы системы MyNUnit.
    /// </summary>
    [NUnit.Framework.TestFixture]
    public class MyNUnitTests
    {
        private StringWriter consoleOutput;

        /// <summary>
        /// Ќастраивает перехват вывода в консоль перед каждым тестом.
        /// </summary>
        [NUnit.Framework.SetUp]
        public void SetUp()
        {
            this.consoleOutput = new StringWriter();
            Console.SetOut(this.consoleOutput);
        }

        /// <summary>
        /// ¬осстанавливает стандартный вывод в консоль и освобождает ресурсы после выполнени€ теста.
        /// </summary>
        [NUnit.Framework.TearDown]
        public void TearDown()
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            this.consoleOutput.Dispose();
        }

        /// <summary>
        /// ѕровер€ет корректность выполнени€ методов с атрибутами BeforeClass и AfterClass.
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
        /// ѕровер€ет корректность выполнени€ методов с атрибутами Before и After перед и после каждого теста.
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
        /// ѕровер€ет, что тест корректно обрабатывает ожидаемое исключение.
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
        /// ѕровер€ет, что игнорируемые тесты не выполн€ютс€ и выводитс€ причина.
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
        /// ѕровер€ет, что успешный тест проходит корректно.
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
        /// ѕровер€ет, что тесты выполн€ютс€ параллельно.
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
        /// ¬спомогательный метод дл€ подсчета числа вхождений подстроки.
        /// </summary>
        /// <param name="text">“екст, в котором выполн€етс€ поиск.</param>
        /// <param name="substring">ѕодстрока дл€ поиска.</param>
        /// <returns> оличество вхождений подстроки в тексте.</returns>
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
