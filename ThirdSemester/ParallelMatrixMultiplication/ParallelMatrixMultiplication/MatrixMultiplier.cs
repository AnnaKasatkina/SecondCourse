// <copyright file="MatrixMultiplier.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

using System.Diagnostics;

/// <summary>
/// Provides methods for multiplying matrices both sequentially and in parallel.
/// </summary>
public static class MatrixMultiplier
{
    /// <summary>
    /// Multiplies two matrices sequentially.
    /// </summary>
    /// <param name="matrixA">The first matrix.</param>
    /// <param name="matrixB">The second matrix.</param>
    /// <returns>A new matrix resulting from the multiplication of matrixA and matrixB.</returns>
    public static int[,] SequentialMultiplyMatrix(int[,] matrixA, int[,] matrixB)
    {
        ValidateMatricesForMultiplication(matrixA, matrixB);

        var rowsA = matrixA.GetLength(0);
        var colsA = matrixA.GetLength(1);
        var colsB = matrixB.GetLength(1);
        var result = new int[rowsA, colsB];

        for (var i = 0; i < rowsA; i++)
        {
            for (var j = 0; j < colsB; j++)
            {
                var sum = 0;
                for (var k = 0; k < colsA; k++)
                {
                    sum += matrixA[i, k] * matrixB[k, j];
                }

                result[i, j] = sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Multiplies two matrices in parallel using multiple threads.
    /// </summary>
    /// <param name="matrixA">The first matrix.</param>
    /// <param name="matrixB">The second matrix.</param>
    /// <returns>A new matrix resulting from the parallel multiplication of matrixA and matrixB.</returns>
    public static int[,] ParallelMultiplyMatrix(int[,] matrixA, int[,] matrixB)
    {
        ValidateMatricesForMultiplication(matrixA, matrixB);

        var rowsA = matrixA.GetLength(0);
        var colsA = matrixA.GetLength(1);
        var colsB = matrixB.GetLength(1);
        var result = new int[rowsA, colsB];

        var threadCount = Math.Min(rowsA, Environment.ProcessorCount);
        var rowsPerThread = (int)Math.Ceiling((double)rowsA / threadCount);
        Thread[] threads = new Thread[threadCount];

        for (var i = 0; i < threadCount; i++)
        {
            var startRow = i * rowsPerThread;
            var endRow = Math.Min(rowsA, startRow + rowsPerThread);

            threads[i] = new Thread(() => MultiplyMatrixRows(matrixA, matrixB, result, startRow, endRow));
            threads[i].Start();
        }

        foreach (var t in threads)
        {
            t.Join();
        }

        return result;
    }

    /// <summary>
    /// Measures the execution time of a given matrix multiplication method.
    /// </summary>
    /// <param name="multiplicationMethod">The matrix multiplication method to measure.</param>
    /// <returns>The elapsed time in milliseconds.</returns>
    public static double MeasureExecutionTime(Action multiplicationMethod)
    {
        var stopwatch = Stopwatch.StartNew();
        multiplicationMethod();
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    private static void MultiplyMatrixRows(int[,] matrixA, int[,] matrixB, int[,] result, int startRow, int endRow)
    {
        var colsA = matrixA.GetLength(1);
        var colsB = matrixB.GetLength(1);

        for (var i = startRow; i < endRow; i++)
        {
            for (var j = 0; j < colsB; j++)
            {
                var sum = 0;
                for (var k = 0; k < colsA; k++)
                {
                    sum += matrixA[i, k] * matrixB[k, j];
                }

                result[i, j] = sum;
            }
        }
    }

    private static void ValidateMatricesForMultiplication(int[,] matrixA, int[,] matrixB)
    {
        if (matrixA.GetLength(1) != matrixB.GetLength(0))
        {
            throw new ArgumentException("Число столбцов первой матрицы должно совпадать с числом строк второй матрицы.");
        }
    }
}
