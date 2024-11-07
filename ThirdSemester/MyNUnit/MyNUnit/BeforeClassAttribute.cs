// <copyright file="BeforeClassAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyNUnit
{
    /// <summary>
    /// Атрибут для обозначения статического метода, который должен выполняться один раз перед всеми тестами в классе.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeClassAttribute : Attribute
    {
    }
}
