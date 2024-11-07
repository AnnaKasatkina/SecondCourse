// <copyright file="AfterAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyNUnit
{
    /// <summary>
    /// Атрибут для обозначения метода, который должен выполняться после каждого теста в классе.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterAttribute : Attribute
    {
    }
}
