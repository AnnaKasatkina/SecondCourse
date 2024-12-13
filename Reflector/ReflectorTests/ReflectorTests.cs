using ReflectorApp;

namespace ReflectorTests;

[TestFixture]
public class ReflectorTests
{
    private Reflector _reflector;

    [SetUp]
    public void Setup()
    {
        _reflector = new Reflector();
    }

    /// <summary>
    /// Проверка корректности сравнения двух классов.
    /// </summary>
    [Test]
    public void TestDiffClasses()
    {
        using var writer = new StringWriter();
        Console.SetOut(writer);

        _reflector.DiffClasses(typeof(TestClass), typeof(TestClass2));

        var output = writer.ToString();
        Assert.IsTrue(output.Contains("Only in TestClass: PrivateField"), "Diff should detect unique field");
        Assert.IsTrue(output.Contains("Only in TestClass2: ExtraField"), "Diff should detect extra field");
    }

    /// <summary>
    /// Проверка на корректную генерацию структуры пустого класса.
    /// </summary>
    [Test]
    public void TestPrintStructureEmptyClass()
    {
        var testClass = typeof(EmptyClass);
        _reflector.PrintStructure(testClass);

        var expectedFileName = $"{testClass.Name}.cs";
        Assert.IsTrue(File.Exists(expectedFileName), "File should be created for empty class");

        var content = File.ReadAllText(expectedFileName);
        Assert.IsTrue(content.Contains("public class EmptyClass"), "Should contain empty class declaration");
        Assert.IsTrue(content.Contains("{"), "Should contain opening brace");
        Assert.IsTrue(content.Contains("}"), "Should contain closing brace");
    }

    public class TestClass
    {
        public int PublicField;
        private int PrivateField;

        public void PublicMethod() { }
        private void PrivateMethod() { }
    }

    public class TestClass2
    {
        public int PublicField;
        private int ExtraField;

        public void PublicMethod() { }
    }

    public class EmptyClass { }
}
