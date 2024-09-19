// <copyright file="MatrixMultiplier.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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

        int rowsA = matrixA.GetLength(0);
        int colsA = matrixA.GetLength(1);
        int colsB = matrixB.GetLength(1);
        int[,] result = new int[rowsA, colsB];

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsB; j++)
            {
                int sum = 0;
                for (int k = 0; k < colsA; k++)
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

        int rowsA = matrixA.GetLength(0);
        int colsA = matrixA.GetLength(1);
        int colsB = matrixB.GetLength(1);
        int[,] result = new int[rowsA, colsB];

        int threadCount = Environment.ProcessorCount;
        int rowsPerThread = rowsA / threadCount;
        Thread[] threads = new Thread[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            int startRow = i * rowsPerThread;
            int endRow = (i == threadCount - 1) ? rowsA : startRow + rowsPerThread;

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
        Stopwatch stopwatch = Stopwatch.StartNew();
        multiplicationMethod();
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    private static void MultiplyMatrixRows(int[,] matrixA, int[,] matrixB, int[,] result, int startRow, int endRow)
    {
        int colsA = matrixA.GetLength(1);
        int colsB = matrixB.GetLength(1);

        for (int i = startRow; i < endRow; i++)
        {
            for (int j = 0; j < colsB; j++)
            {
                int sum = 0;
                for (int k = 0; k < colsA; k++)
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
