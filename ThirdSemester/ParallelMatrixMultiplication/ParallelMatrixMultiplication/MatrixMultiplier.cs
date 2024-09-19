using System.Diagnostics;

public static class MatrixMultiplier
{
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

    public static double MeasureExecutionTime(Action multiplicationMethod)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        multiplicationMethod();
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    private static void ValidateMatricesForMultiplication(int[,] matrixA, int[,] matrixB)
    {
        if (matrixA.GetLength(1) != matrixB.GetLength(0))
        {
            throw new ArgumentException("Число столбцов первой матрицы должно совпадать с числом строк второй матрицы.");
        }
    }

}