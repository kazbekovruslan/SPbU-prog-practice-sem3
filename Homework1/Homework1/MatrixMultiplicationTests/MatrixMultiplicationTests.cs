namespace MatrixMultiplicationTests;

public class Tests
{
    private static IEnumerable<TestCaseData> BadTestFiles()
    {
        yield return new TestCaseData("../../../TestFiles/MatrixWithExtraElements.txt");
        yield return new TestCaseData("../../../TestFiles/NotMatrix.txt");
        yield return new TestCaseData("../../../TestFiles/EmptyMatrix.txt");
        yield return new TestCaseData("../../../TestFiles/MatrixWithSpaces.txt");
    }

    private static IEnumerable<TestCaseData> MatricesForMultiplication()
    {
        yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 },
                    { 7, 8, 9 }
                }),
                new Matrix(new int[,]
                {
                    { 1, 2, 3 },
                    { 3, 4, 5 },
                    { 5, 6, 7 }
                }),
                new Matrix(new int[,]
                {
                    { 22, 28, 34 },
                    { 49, 64, 79 },
                    { 76, 100, 124 } })
                );
    }

    private static IEnumerable<TestCaseData> MatricesWithDifferentDimensions()
    {
        yield return new TestCaseData(
                new Matrix(new int[,]
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 },
                    { 7, 8, 9 }
                }),
                new Matrix(new int[,]
                {
                    { 1, 2 },
                    { 3, 4 },
                }));
    }

    [TestCaseSource(nameof(BadTestFiles))]
    public void MatrixConstructorWithBadMatricesShouldThrowException(string pathToFile)
        => Assert.Throws<ArgumentException>(() => new Matrix(pathToFile));


    [Test]
    public void MatrixConstructorWithWrongPathToFileShouldThrowException()
        => Assert.Throws<FileNotFoundException>(() => new Matrix("WrongPathToFile.txt"));

    [TestCaseSource(nameof(MatricesWithDifferentDimensions))]
    public void MatrixMultiplyOfMatricesWithDifferentDimensionsShouldThrowException(Matrix matrix1, Matrix matrix2)
        => Assert.Throws<ArgumentException>(() => Matrix.Multiply(matrix1, matrix2));

    [TestCaseSource(nameof(MatricesWithDifferentDimensions))]
    public void MatrixParallelMultiplyOfMatricesWithDifferentDimensionsShouldThrowException(Matrix matrix1, Matrix matrix2)
        => Assert.Throws<ArgumentException>(() => Matrix.ParallelMultiply(matrix1, matrix2));

    [TestCaseSource(nameof(MatricesForMultiplication))]
    public void MatrixMultiplyOfCommonMatricesShouldReturnRightAnswer(Matrix matrix1, Matrix matrix2, Matrix expectedResult)
        => Assert.That(Matrix.Multiply(matrix1, matrix2).IsEqualTo(expectedResult), Is.True);

    [TestCaseSource(nameof(MatricesForMultiplication))]
    public void MatrixParallelMultiplyOfCommonMatricesShouldReturnRightAnswer(Matrix matrix1, Matrix matrix2, Matrix expectedResult)
        => Assert.That(Matrix.ParallelMultiply(matrix1, matrix2).IsEqualTo(expectedResult), Is.True);

}