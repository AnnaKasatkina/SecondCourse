// <copyright file="SimpleLazy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// A lazy evaluation implementation for single-threaded environments.
/// The value is computed only once during the first call to Get.
/// </summary>
/// <typeparam name="T">The type of the computed value.</typeparam>
public class SimpleLazy<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T? value = default;
    private bool isEvaluated = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">
    /// The function that supplies the value to be lazily evaluated.
    /// The supplier function will only be invoked once, upon the first call to <see cref="Get"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="supplier"/> is null.
    /// </exception>
    public SimpleLazy(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
    }

    /// <summary>
    /// Returns the computed value. The computation occurs only on the first call.
    /// </summary>
    /// <returns>The computed value, which may be null.</returns>
    public T Get()
    {
        if (!this.isEvaluated)
        {
            if (this.supplier == null)
            {
                throw new InvalidOperationException("Supplier is null");
            }

            this.value = this.supplier();
            this.isEvaluated = true;
            this.supplier = null;
        }

        return this.value!;
    }
}