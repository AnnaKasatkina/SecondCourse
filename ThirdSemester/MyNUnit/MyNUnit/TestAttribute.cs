// <copyright file="TestAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Attribute to mark a method as a test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the type of exception expected when executing the test.
    /// </summary>
    public Type? Expected { get; set; }

    /// <summary>
    /// Gets or sets the reason why the test should be ignored.
    /// </summary>
    public string? Ignore { get; set; }
}
