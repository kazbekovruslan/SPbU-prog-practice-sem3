namespace MatrixMultiplication;

using System.Text.RegularExpressions;

/// <summary>
/// Class that represents matrix and operations on it
/// </summary>
public class Matrix
{
    /// <summary>
    /// Amount of rows in matrix.
    /// </summary>
    public int RowsAmount { get; }

    /// <summary>
    /// Amount of columns in matrix.
    /// </summary>
    public int ColumnsAmount { get; }

    /// <summary>
    /// Two-dimensional array that contains rows of matrix.
    /// </summary>
    public int[,] MatrixElements { get; }

    /// <summary>
    /// Constructor of Matrix from file.
    /// </summary>
    /// <param name="matrixElements">Matrix's elements.</param>
    public Matrix(string pathToFile)
    {
        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException($"File '{pathToFile}' not found!");
        }

        string[] allLines = File.ReadAllLines(pathToFile);

        int rowsAmount = allLines.Length;
        var matrix = new List<int[]>();
        var columnsAmount = 0;

        foreach (var line in allLines)
        {
            var row = new Regex(@"-?\d+", RegexOptions.Compiled).Matches(line)
                                    .Select(element => int.Parse(element.Value)).ToArray();

            if (columnsAmount != 0 && row.Length != columnsAmount)
            {
                throw new ArgumentException("It is not matrix in your file!");
            }

            columnsAmount = row.Length;
            matrix.Add(row);
        }

        if (matrix.Count == 0)
        {
            throw new ArgumentException("File is empty!");
        }

        var matrixElements = new int[rowsAmount, columnsAmount];
        ColumnsAmount = columnsAmount;
        RowsAmount = rowsAmount;
        for (var i = 0; i < rowsAmount; ++i)
        {
            for (var j = 0; j < columnsAmount; ++j)
            {
                matrixElements[i, j] = matrix[i][j];
            }
        }

        MatrixElements = matrixElements;
    }

    /// <summary>
    /// Constructor of Matrix from two-dimensional array.
    /// </summary>
    /// <param name="matrixElements">Matrix's elements.</param>
    public Matrix(int[,] matrixElements)
    {
        MatrixElements = (int[,])matrixElements.Clone();
        RowsAmount = matrixElements.GetLength(0);
        ColumnsAmount = matrixElements.GetLength(1);
    }

    /// <summary>
    /// Writes matrix in file.
    /// </summary>
    /// <param name="pathToFile">Path to file where the matrix will be written.</param>
    public void WriteInFile(string pathToFile)
    {
        using (var writer = new StreamWriter(pathToFile))
        {
            for (var i = 0; i < RowsAmount; ++i)
            {
                for (var j = 0; j < ColumnsAmount; ++j)
                {
                    writer.Write($"{MatrixElements[i, j]} ");
                }
                writer.Write("\n");
            }
        }
    }

    /// <summary>
    /// Multiplies matrices.
    /// </summary>
    /// <param name="firstMatrix">First matrix for multiplying.</param>
    /// <param name="secondMatrix">Second matrix for multiplying.</param>
    /// <returns>Matrix obtained by multiplication of matrices.</returns>
    public static Matrix Multiply(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.ColumnsAmount != secondMatrix.RowsAmount)
        {
            throw new ArgumentException("Dimensions are not suitable for multiplication!");
        }

        var newMatrix = new int[firstMatrix.RowsAmount, secondMatrix.ColumnsAmount];

        for (int i = 0; i < firstMatrix.RowsAmount; ++i)
        {
            for (int j = 0; j < secondMatrix.ColumnsAmount; ++j)
            {
                for (int k = 0; k < firstMatrix.ColumnsAmount; ++k)
                {
                    newMatrix[i, j] += firstMatrix.MatrixElements[i, k] * secondMatrix.MatrixElements[k, j];
                }
            }
        }

        return new Matrix(newMatrix);
    }

    /// <summary>
    /// Multiplies matrices in parallel.
    /// </summary>
    /// <param name="firstMatrix">First matrix for multiplying.</param>
    /// <param name="secondMatrix">Second matrix for multiplying.</param>
    /// <returns>Matrix obtained by multiplication of matrices.</returns>
    public static Matrix ParallelMultiply(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.ColumnsAmount != secondMatrix.RowsAmount)
        {
            throw new ArgumentException("Dimensions are not suitable for multiplication!");
        }

        var newMatrix = new int[firstMatrix.RowsAmount, secondMatrix.ColumnsAmount];

        var threads = new Thread[firstMatrix.ColumnsAmount];
        for (int i = 0; i < firstMatrix.RowsAmount; ++i)
        {
            var local = i;
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < secondMatrix.ColumnsAmount; ++j)
                {
                    var sumForElement = 0;
                    for (int k = 0; k < firstMatrix.ColumnsAmount; ++k)
                    {
                        sumForElement += firstMatrix.MatrixElements[local, k] * secondMatrix.MatrixElements[k, j];
                    }
                    newMatrix[local, j] = sumForElement;
                }
            });
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return new Matrix(newMatrix);
    }
}