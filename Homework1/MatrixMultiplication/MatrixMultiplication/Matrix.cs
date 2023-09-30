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
    public int RowsAmount
        => MatrixElements.GetLength(0);

    /// <summary>
    /// Amount of columns in matrix.
    /// </summary>
    public int ColumnsAmount
        => MatrixElements.GetLength(1);

    /// <summary>
    /// Two-dimensional array that contains rows of matrix.
    /// </summary>
    public int[,] MatrixElements { get; }

    /// <summary>
    /// Constructor of Matrix from two-dimensional array.
    /// </summary>
    /// <param name="matrixElements">Matrix's elements.</param>
    public Matrix(int[,] matrixElements)
        => MatrixElements = (int[,])matrixElements.Clone();

    /// <summary>
    /// Constructor of Matrix from file.
    /// </summary>
    /// <param name="pathToFile">Path to file that contain matrix.</param>
    public Matrix(string pathToFile)
        => MatrixElements = ReadMatrixFromFile(pathToFile);

    /// <summary>
    /// Reads matrix from file.
    /// </summary>
    /// <param name="pathToFile">Path to file that contain matrix.</param>
    /// <returns>Two dimensional array - matrix.</returns>
    public int[,] ReadMatrixFromFile(string pathToFile)
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
                throw new ArgumentException($"It is not matrix in your file '{pathToFile}'!");
            }

            columnsAmount = row.Length;
            matrix.Add(row);
        }

        if (matrix.Count == 0)
        {
            throw new ArgumentException("File is empty!");
        }

        var matrixElements = new int[rowsAmount, columnsAmount];
        for (var i = 0; i < rowsAmount; ++i)
        {
            for (var j = 0; j < columnsAmount; ++j)
            {
                matrixElements[i, j] = matrix[i][j];
            }
        }

        return matrixElements;
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
                newMatrix[i, j] = Enumerable.Range(0, firstMatrix.ColumnsAmount).
                Sum(k => firstMatrix.MatrixElements[i, k] * secondMatrix.MatrixElements[k, j]);
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

        var processorAmount = Environment.ProcessorCount;
        var threads = new Thread[processorAmount];

        int blockSize = firstMatrix.RowsAmount / processorAmount;
        for (int i = 0; i < processorAmount; i++)
        {
            int rowStart = i * blockSize;
            int rowEnd = (i == processorAmount - 1) ? firstMatrix.RowsAmount : rowStart + blockSize;

            threads[i] = new Thread(() =>
            {
                for (int j = rowStart; j < rowEnd; j++)
                {
                    for (int k = 0; k < secondMatrix.ColumnsAmount; k++)
                    {
                        for (int l = 0; l < firstMatrix.ColumnsAmount; l++)
                        {
                            newMatrix[j, k] += firstMatrix.MatrixElements[j, l] * secondMatrix.MatrixElements[l, k];
                        }
                    }
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


    /// <summary>
    /// Checks if two matrices are equal.
    /// </summary>
    /// <param name="otherMatrix">Second matrix.</param>
    /// <returns>true - if matrices are equal; false - if matrices aren't equal.</returns>
    public bool IsEqualTo(Matrix otherMatrix)
    {
        if (System.Object.ReferenceEquals(this, otherMatrix))
        {
            return true;
        }

        if (otherMatrix == null)
        {
            return false;
        }

        if (RowsAmount != otherMatrix.RowsAmount || ColumnsAmount != otherMatrix.ColumnsAmount)
        {
            return false;
        }

        for (int i = 0; i < RowsAmount; ++i)
        {
            for (int j = 0; j < ColumnsAmount; ++j)
            {
                if (MatrixElements[i, j] != otherMatrix.MatrixElements[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Generates matrix with given dimension and random numbers.
    /// </summary>
    /// <param name="rowsAmount">Amount of rows in matrix.</param>
    /// <param name="columnsAmount">Amount of columns in matrix.</param>
    /// <returns>Matrix with given dimension and random numbers.</returns>
    public static Matrix GenerateRandomMatrix(int rowsAmount, int columnsAmount)
    {
        var rand = new Random();

        var matrixElements = new int[rowsAmount, columnsAmount];
        for (int i = 0; i < rowsAmount; ++i)
        {
            for (int j = 0; j < columnsAmount; ++j)
            {
                matrixElements[i, j] = rand.Next(100);
            }
        }

        return new Matrix(matrixElements);
    }
}