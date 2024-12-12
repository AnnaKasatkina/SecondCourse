// <copyright file="AfterClassAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Attribute to mark a static method that should run once after all tests in a class.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute
{
}
