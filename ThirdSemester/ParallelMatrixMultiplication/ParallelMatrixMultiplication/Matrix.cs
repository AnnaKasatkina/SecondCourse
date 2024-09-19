public static class Matrix
{
    public static int[,] ReadMatrixFromFile(string fileName)
    {
        string[] lines = File.ReadAllLines(fileName);

        if (lines.Length == 0)
        {
            throw new ArgumentException("Файл пуст или содержит некорректные данные.");
        }

        int rows = lines.Length;
        int cols = lines[0].Split(' ').Length;
        int[,] matrix = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            string[] elements = lines[i].Split(' ');

            if (elements.Length != cols)
            {
                throw new ArgumentException("Неверная структура матрицы");
            }

            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = int.Parse(elements[j]);
            }
        }

        return matrix;
    }

    public static void WriteMatrixToFile(string fileName, int[,] matrix)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    writer.Write(matrix[i, j] + (j == cols - 1 ? "" : " "));
                }
                writer.WriteLine();
            }
        }
    }

    public static int[,] GenerateMatrix( int rows,  int cols, int minValue = 0, int maxValue = 100)
    {
        Random random = new Random();
        int[,] matrix = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i,j] = random.Next(minValue, maxValue);
            }
        }
        return matrix;
    }
}
