// <copyright file="BeforeAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Attribute to mark a method to run before each test in a class.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeAttribute : Attribute
{
}
