// <copyright file="BeforeAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyNUnit
{
    /// <summary>
    /// Атрибут для обозначения метода, который должен выполняться перед каждым тестом в классе.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeAttribute : Attribute
    {
    }
}
