// <copyright file="TestRunner.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using MyNUnit;

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

        ValidateStaticMethods(beforeClassMethods, "BeforeClass");
        ValidateStaticMethods(afterClassMethods, "AfterClass");

        ExecuteMethods(beforeClassMethods, null);

        var exceptions = new ConcurrentBag<string>();
        Parallel.ForEach(testMethods, testMethod =>
        {
        var testAttribute = testMethod.GetCustomAttribute<TestAttribute>();
        if (testAttribute.Ignore != null)
        {
            Console.WriteLine($"{testMethod.Name} Ignored: {testAttribute.Ignore}");
            return;
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
                    exceptions.Add($"{testMethod.Name} Failed: Expected exception of type {testAttribute.Expected}");
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
                    exceptions.Add($"{testMethod.Name} Failed: {ex.InnerException} in {stopwatch.ElapsedMilliseconds}ms");
                }
            }

            ExecuteMethods(afterMethods, instance);
        });

        foreach (var exception in exceptions)
        {
            Console.WriteLine(exception);
        }

        ExecuteMethods(afterClassMethods, null);
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

    /// <summary>
    /// Validates that all provided methods are static.
    /// </summary>
    /// <param name="methods">Methods to validate.</param>
    /// <param name="attributeName">Name of the attribute for error reporting.</param>
    private static void ValidateStaticMethods(IEnumerable<MethodInfo> methods, string attributeName)
    {
        foreach (var method in methods)
        {
            if (!method.IsStatic)
            {
                throw new InvalidOperationException($"Method {method.Name} marked with {attributeName} must be static.");
            }
        }
    }
}