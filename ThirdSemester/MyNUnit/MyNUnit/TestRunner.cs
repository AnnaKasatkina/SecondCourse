// <copyright file="TestRunner.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Reflection;
using MyNUnit;

/// <summary>
/// Класс, отвечающий за запуск тестов и выполнение методов с атрибутами BeforeClass, AfterClass, Before, After и Test.
/// </summary>
public class TestRunner
{
    /// <summary>
    /// Выполняет тесты из всех сборок, найденных в указанной директории.
    /// </summary>
    /// <param name="path">Путь к директории, содержащей сборки с тестами.</param>
    public static void RunTests(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var assemblies = Directory.EnumerateFiles(path, "*.dll")
                                  .Select(Assembly.LoadFrom);

        foreach (var assembly in assemblies)
        {
            var testClasses = assembly.GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any()));

            Parallel.ForEach(testClasses, testClass =>
            {
                RunTestClass(testClass);
            });
        }
    }

    /// <summary>
    /// Выполняет тесты внутри одного класса, управляет вызовом методов BeforeClass, AfterClass, Before, After и Test.
    /// </summary>
    /// <param name="testClass">Тип тестируемого класса.</param>
    private static void RunTestClass(Type testClass)
    {
        var beforeClassMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(BeforeClassAttribute), false).Any());
        var afterClassMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(AfterClassAttribute), false).Any());
        var beforeMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(BeforeAttribute), false).Any());
        var afterMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(AfterAttribute), false).Any());
        var testMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any());

        ExecuteMethods(beforeClassMethods, null);

        foreach (var testMethod in testMethods)
        {
            var testAttribute = testMethod.GetCustomAttribute<TestAttribute>();
            if (testAttribute.Ignore != null)
            {
                Console.WriteLine($"{testMethod.Name} Ignored: {testAttribute.Ignore}");
                continue;
            }

            var instance = Activator.CreateInstance(testClass);
            ExecuteMethods(beforeMethods, instance);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                testMethod.Invoke(instance, null);
                stopwatch.Stop();

                if (testAttribute.Expected != null)
                {
                    Console.WriteLine($"{testMethod.Name} Failed: Expected exception of type {testAttribute.Expected}");
                }
                else
                {
                    Console.WriteLine($"{testMethod.Name} Passed in {stopwatch.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                if (testAttribute.Expected != null && ex.InnerException?.GetType() == testAttribute.Expected)
                {
                    Console.WriteLine($"{testMethod.Name} Passed (Expected exception) in {stopwatch.ElapsedMilliseconds}ms");
                }
                else
                {
                    Console.WriteLine($"{testMethod.Name} Failed: {ex.InnerException} in {stopwatch.ElapsedMilliseconds}ms");
                }
            }

            ExecuteMethods(afterMethods, instance);
        }

        ExecuteMethods(afterClassMethods, null);
    }

    /// <summary>
    /// Выполняет набор методов.
    /// </summary>
    /// <param name="methods">Список методов для выполнения.</param>
    /// <param name="instance">Экземпляр класса, у которого выполняются методы, или null для статических методов.</param>
    private static void ExecuteMethods(IEnumerable<MethodInfo>? methods, object? instance)
    {
        foreach (var method in methods ?? Enumerable.Empty<MethodInfo>())
        {
            method.Invoke(instance, null);
        }
    }
}