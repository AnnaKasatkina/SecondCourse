class Program
{
    static void Main(string[] args)
    {
        int[] testCasesRowsA = { 100, 500, 1000, 1500 };
        int[] testCasesColsA = { 100, 500, 1000, 1500 };
        int[] testCasesColsB = { 200, 400, 800, 1500 };
        int numRuns = 5;

        Console.WriteLine($"{Format("Размеры матриц", 40)}{Format("Среднее время (последовательное)", 40)}{Format("Среднекв. отклонение", 40)}{Format("Среднее время (параллельное)", 40)}{Format("Среднекв. отклонение", 40)}");

        for (int i = 0; i < testCasesRowsA.Length; i++)
        {
            int rowsA = testCasesRowsA[i];
            int colsA = testCasesColsA[i];
            int colsB = testCasesColsB[i];

            int[,] matrixA = Matrix.GenerateMatrix(rowsA, colsA);
            int[,] matrixB = Matrix.GenerateMatrix(colsA, colsB);

            List<double> sequentialTimes = new List<double>();
            for (int run = 0; run < numRuns; run++)
            {
                double sequentialTime = MatrixMultiplier.MeasureExecutionTime(() =>
                {
                    var result = MatrixMultiplier.SequentialMultiplyMatrix(matrixA, matrixB);
                });
                sequentialTimes.Add(sequentialTime);
            }

            List<double> parallelTimes = new List<double>();
            for (int run = 0; run < numRuns; run++)
            {
                double parallelTime = MatrixMultiplier.MeasureExecutionTime(() =>
                {
                    var result = MatrixMultiplier.ParallelMultiplyMatrix(matrixA, matrixB);
                });
                parallelTimes.Add(parallelTime);
            }

            double sequentialMean = sequentialTimes.Average();
            double sequentialStdDev = Math.Sqrt(sequentialTimes.Average(v => Math.Pow(v - sequentialMean, 2)));
            double parallelMean = parallelTimes.Average();
            double parallelStdDev = Math.Sqrt(parallelTimes.Average(v => Math.Pow(v - parallelMean, 2)));

            Console.WriteLine($"{Format($"{rowsA}x{colsA} и {colsA}x{colsB}", 40)}{Format($"{sequentialMean:F3} ms", 40)}{Format($"{sequentialStdDev:F3} ms", 40)}{Format($"{parallelMean:F3} ms", 40)}{Format($"{parallelStdDev:F3} ms", 40)}");
        }
    }

    static string Format(string text, int width)
    {
        return text.PadRight(width);
    }
}
