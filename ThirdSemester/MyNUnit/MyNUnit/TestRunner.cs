// <copyright file="TestRunner.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using MyNUnit.Attributes;

/// <summary>
/// The main class responsible for running tests and executing methods marked with BeforeClass, AfterClass, Before, After, and Test attributes.
/// </summary>
public class TestRunner
{
    /// <summary>
    /// Executes tests from all assemblies found in the specified directory.
    /// </summary>
    /// <param name="path">Path to the directory containing assemblies with tests.</param>
    public static void RunTests(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

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
    /// Executes tests within a single class, manages the invocation of BeforeClass, AfterClass, Before, After, and Test methods.
    /// </summary>
    /// <param name="testClass">The type of the class being tested.</param>
    private static void RunTestClass(Type testClass)
    {
        var beforeClassMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(BeforeClassAttribute), false).Any()).ToList();
        var afterClassMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(AfterClassAttribute), false).Any()).ToList();
        var beforeMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(BeforeAttribute), false).Any()).ToList();
        var afterMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(AfterAttribute), false).Any()).ToList();
        var testMethods = testClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any()).ToList();

        ValidateMethods(beforeClassMethods, "BeforeClass", shouldBeStatic: true);
        ValidateMethods(afterClassMethods, "AfterClass", shouldBeStatic: true);
        ValidateMethods(beforeMethods, "Before", shouldBeStatic: false);
        ValidateMethods(afterMethods, "After", shouldBeStatic: false);

        var testResults = new ConcurrentBag<string>();

        bool beforeClassSucceeded = true;
        try
        {
            ExecuteMethods(beforeClassMethods, null);
        }
        catch (Exception ex)
        {
            beforeClassSucceeded = false;
            string message = $"BeforeClass failed: {ex.InnerException?.Message ?? ex.Message}";
            foreach (var testMethod in testMethods)
            {
                testResults.Add($"{testMethod.Name} Errored: {message}");
            }
        }

        if (!beforeClassSucceeded)
        {
            try
            {
                ExecuteMethods(afterClassMethods, null);
            }
            catch (Exception ex)
            {
                testResults.Add($"AfterClass failed: {ex.InnerException?.Message ?? ex.Message}");
            }

            PrintResultsForClass(testClass, testResults);
            return;
        }

        Parallel.ForEach(testMethods, testMethod =>
        {
            var testAttribute = testMethod.GetCustomAttribute<TestAttribute>();

            if (testAttribute.Ignore != null)
            {
                testResults.Add($"{testMethod.Name} Ignored: {testAttribute.Ignore}");
                return;
            }

            var instance = Activator.CreateInstance(testClass);

            try
            {
                ExecuteMethods(beforeMethods, instance);
            }
            catch (Exception ex)
            {
                testResults.Add($"{testMethod.Name} Errored (Before failed): {ex.InnerException?.Message ?? ex.Message}");
                try
                {
                    ExecuteMethods(afterMethods, instance);
                }
                catch (Exception afterEx)
                {
                    testResults.Add($"{testMethod.Name} Errored (After failed): {afterEx.InnerException?.Message ?? afterEx.Message}");
                }

                return;
            }

            var stopwatch = Stopwatch.StartNew();
            try
            {
                testMethod.Invoke(instance, null);
                stopwatch.Stop();

                if (testAttribute.Expected != null)
                {
                    testResults.Add($"{testMethod.Name} Failed: Expected exception of type {testAttribute.Expected.Name} in {stopwatch.ElapsedMilliseconds}ms");
                }
                else
                {
                    testResults.Add($"{testMethod.Name} Passed in {stopwatch.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                if (testAttribute.Expected != null &&
                    ex.InnerException != null &&
                    ex.InnerException.GetType() == testAttribute.Expected)
                {
                    testResults.Add($"{testMethod.Name} Passed (Expected exception) in {stopwatch.ElapsedMilliseconds}ms");
                }
                else
                {
                    testResults.Add($"{testMethod.Name} Failed: {ex.InnerException?.Message ?? ex.Message} in {stopwatch.ElapsedMilliseconds}ms");
                }
            }

            try
            {
                ExecuteMethods(afterMethods, instance);
            }
            catch (Exception ex)
            {
                testResults.Add($"{testMethod.Name} Errored (After failed): {ex.InnerException?.Message ?? ex.Message}");
            }
        });

        try
        {
            ExecuteMethods(afterClassMethods, null);
        }
        catch (Exception ex)
        {
            string afterClassError = $"AfterClass failed: {ex.InnerException?.Message ?? ex.Message}";
            testResults.Add(afterClassError);
        }

        PrintResultsForClass(testClass, testResults);
    }

    /// <summary>
    /// Executes a set of methods.
    /// </summary>
    /// <param name="methods">List of methods to execute.</param>
    /// <param name="instance">The instance of the class for non-static methods, or null for static methods.</param>
    private static void ExecuteMethods(IEnumerable<MethodInfo>? methods, object? instance)
    {
        foreach (var method in methods ?? Enumerable.Empty<MethodInfo>())
        {
            method.Invoke(instance, null);
        }
    }

    private static void ValidateMethods(IEnumerable<MethodInfo> methods, string attributeName, bool shouldBeStatic)
    {
        foreach (var method in methods)
        {
            if (method.IsStatic != shouldBeStatic)
            {
                throw new InvalidOperationException(
                    $"Method {method.Name} with {attributeName} must {(shouldBeStatic ? "be static" : "not be static")}");
            }

            if (method.ReturnType != typeof(void))
            {
                throw new InvalidOperationException(
                    $"Method {method.Name} with {attributeName} must return void");
            }

            if (method.GetParameters().Length > 0)
            {
                throw new InvalidOperationException(
                    $"Method {method.Name} with {attributeName} must be parameterless");
            }
        }
    }

    /// <summary>
    /// Выводит группой результаты тестирования для данного класса.
    /// </summary>
    /// <param name="testClass">Тип тестового класса.</param>
    /// <param name="results">Коллекция результатов тестов.</param>
    private static void PrintResultsForClass(Type testClass, ConcurrentBag<string> results)
    {
        Console.WriteLine($"----- Results for class {testClass.FullName} -----");
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        Console.WriteLine("--------------------------------------------------");
    }
}