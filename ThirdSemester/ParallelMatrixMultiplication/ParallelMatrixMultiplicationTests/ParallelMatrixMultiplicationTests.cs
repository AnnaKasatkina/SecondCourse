[TestFixture]
public class ParallelMatrixMultiplicationTests
{
    [Test]
    public void TestSquareMatrixMultiplication()
    {
        int[,] matrixA = { { 1, 2 }, { 3, 4 } };
        int[,] matrixB = { { 2, 0 }, { 1, 2 } };
        int[,] expected = { { 4, 4 }, { 10, 8 } };

        var result = MatrixMultiplier.SequentialMultiplyMatrix(matrixA, matrixB);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestRectangularMatrixMultiplication()
    {
        int[,] matrixA = { { 1, 2, 3 }, { 4, 5, 6 } };
        int[,] matrixB = { { 7, 8 }, { 9, 10 }, { 11, 12 } };
        int[,] expected = { { 58, 64 }, { 139, 154 } };

        var result = MatrixMultiplier.SequentialMultiplyMatrix(matrixA, matrixB);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestIdentityMatrixMultiplication()
    {
        int[,] matrixA = { { 1, 0 }, { 0, 1 } };
        int[,] matrixB = { { 5, 6 }, { 7, 8 } };
        int[,] expected = { { 5, 6 }, { 7, 8 } };

        var result = MatrixMultiplier.SequentialMultiplyMatrix(matrixA, matrixB);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestZeroMatrixMultiplication()
    {
        int[,] matrixA = { { 1, 2 }, { 3, 4 } };
        int[,] matrixB = { { 0, 0 }, { 0, 0 } };
        int[,] expected = { { 0, 0 }, { 0, 0 } };

        var result = MatrixMultiplier.SequentialMultiplyMatrix(matrixA, matrixB);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestInvalidMatrixMultiplication()
    {
        int[,] matrixA = { { 1, 2, 3 }, { 4, 5, 6 } };
        int[,] matrixB = { { 1, 2 } };

        Assert.Throws<ArgumentException>(() => MatrixMultiplier.SequentialMultiplyMatrix(matrixA, matrixB));
    }
}
