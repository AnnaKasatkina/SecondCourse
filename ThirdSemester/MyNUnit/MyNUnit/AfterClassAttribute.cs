// <copyright file="AfterClassAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyNUnit
{
    /// <summary>
    /// Атрибут для обозначения статического метода, который должен выполняться один раз после всех тестов в классе.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterClassAttribute : Attribute
    {
    }
}
