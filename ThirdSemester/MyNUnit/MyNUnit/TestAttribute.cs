// <copyright file="TestAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace MyNUnit
{
    /// <summary>
    /// Атрибут для обозначения метода как тестового.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Тип исключения, который ожидается при выполнении теста.
        /// </summary>
        public Type? Expected { get; set; }

        /// <summary>
        /// Причина, по которой тест должен быть проигнорирован.
        /// </summary>
        public string? Ignore { get; set; }
    }
}
