// <copyright file="Matrix.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Provides utility functions for reading, writing, and generating matrices.
/// </summary>
public static class Matrix
{
    /// <summary>
    /// Reads a matrix from a file.
    /// </summary>
    /// <param name="fileName">The path to the file containing the matrix.</param>
    /// <returns>A matrix represented as a 2D array.</returns>
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

    /// <summary>
    /// Writes a matrix to a file.
    /// </summary>
    /// <param name="fileName">The path to the output file.</param>
    /// <param name="matrix">The matrix to write to the file.</param>
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
                    writer.Write(matrix[i, j] + (j == cols - 1 ? string.Empty : " "));
                }

                writer.WriteLine();
            }
        }
    }

    /// <summary>
    /// Generates a random matrix.
    /// </summary>
    /// <param name="rows">The number of rows in the matrix.</param>
    /// <param name="cols">The number of columns in the matrix.</param>
    /// <param name="minValue">The minimum value for each element.</param>
    /// <param name="maxValue">The maximum value for each element.</param>
    /// <returns>A randomly generated matrix.</returns>
    public static int[,] GenerateMatrix(int rows, int cols, int minValue = 0, int maxValue = 100)
    {
        Random random = new Random();
        int[,] matrix = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = random.Next(minValue, maxValue);
            }
        }

        return matrix;
    }
}
