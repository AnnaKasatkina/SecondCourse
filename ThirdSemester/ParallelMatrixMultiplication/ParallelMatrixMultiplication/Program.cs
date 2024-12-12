// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Benchmarks matrix multiplication methods.
/// </summary>

int[] testCasesRowsA = { 100, 500, 1000, 1500 };
int[] testCasesColsA = { 100, 500, 1000, 1500 };
int[] testCasesColsB = { 200, 400, 800, 1500 };
int numRuns = 5;

Console.WriteLine($"{Format("Размеры матриц", 40)}{Format("Среднее время (последовательное)", 40)}{Format("Среднекв. отклонение", 40)}{Format("Среднее время (параллельное)", 40)}{Format("Среднекв. отклонение", 40)}");

for (var i = 0; i < testCasesRowsA.Length; i++)
{
    var rowsA = testCasesRowsA[i];
    var colsA = testCasesColsA[i];
    var colsB = testCasesColsB[i];

    var matrixA = ParallelMatrixMultiplication.Matrix.GenerateMatrix(rowsA, colsA);
    var matrixB = ParallelMatrixMultiplication.Matrix.GenerateMatrix(colsA, colsB);

    var sequentialTimes = new List<double>(numRuns);
    for (var run = 0; run < numRuns; run++)
    {
        var sequentialTime = ParallelMatrixMultiplication.MatrixMultiplier.MeasureExecutionTime(() =>
        {
            var result = ParallelMatrixMultiplication.MatrixMultiplier.SequentialMultiplyMatrix(matrixA, matrixB);
        });
        sequentialTimes.Add(sequentialTime);
    }

    var parallelTimes = new List<double>(numRuns);
    for (var run = 0; run < numRuns; run++)
    {
        var parallelTime = ParallelMatrixMultiplication.MatrixMultiplier.MeasureExecutionTime(() =>
        {
            var result = ParallelMatrixMultiplication.MatrixMultiplier.ParallelMultiplyMatrix(matrixA, matrixB);
        });
        parallelTimes.Add(parallelTime);
    }

    var sequentialMean = sequentialTimes.Average();
    var sequentialStdDev = Math.Sqrt(sequentialTimes.Average(v => Math.Pow(v - sequentialMean, 2)));
    var parallelMean = parallelTimes.Average();
    var parallelStdDev = Math.Sqrt(parallelTimes.Average(v => Math.Pow(v - parallelMean, 2)));

    Console.WriteLine($"{Format($"{rowsA}x{colsA} и {colsA}x{colsB}", 40)}{Format($"{sequentialMean:F3} ms", 40)}{Format($"{sequentialStdDev:F3} ms", 40)}{Format($"{parallelMean:F3} ms", 40)}{Format($"{parallelStdDev:F3} ms", 40)}");
}

static string Format(string text, int width) => text.PadRight(width);
