// <copyright file="ILazy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Interface for lazy evaluation of a value.
/// </summary>
/// <typeparam name="T">The type of the computed value.</typeparam>
public interface ILazy<out T>
{
    /// <summary>
    /// Returns the computed value. The computation occurs only on the first call.
    /// Subsequent calls return the same value.
    /// </summary>
    /// <returns>The computed value.</returns>
    T Get();
}
