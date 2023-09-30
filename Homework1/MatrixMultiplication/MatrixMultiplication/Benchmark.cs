namespace MatrixMultiplication;

using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;

public static class MatrixBenchmark
{
    private const int AmountOfMeasurementsForOneDimension = 20;

    private const int AmountOfTestMeasurements = 5;

    public static void WriteResultsOfBenchmarkingToFile()
    {
        var dimensions = new int[AmountOfTestMeasurements] { 100, 200, 400, 800, 1600 };
        (double, double)[,] resultsOfBenchmarking = MatrixMultiplyBenchmark(dimensions);

        var document = new Document();
        PdfWriter.GetInstance(document, new FileStream("results.pdf", FileMode.Create));
        document.Open();

        var table = new PdfPTable(4);

        var cell = new PdfPCell(new Phrase("Speed measurements of matrix multiplication and parallel matrix multiplication operations"))
        {
            Colspan = 4,
            HorizontalAlignment = Element.ALIGN_CENTER
        };
        table.AddCell(cell);

        table.AddCell("Matrix sizes");
        table.AddCell("Sequentially/in parallel");
        table.AddCell("Math expectation\nin miliseconds");
        table.AddCell("Standart deviation\nin miliseconds");

        for (int i = 0; i < AmountOfTestMeasurements; i++)
        {
            table.AddCell(dimensions[i].ToString() + " x " + dimensions[i].ToString());
            table.AddCell("Sequentially");
            table.AddCell(resultsOfBenchmarking[i, 0].Item1.ToString());
            table.AddCell(resultsOfBenchmarking[i, 0].Item2.ToString());
            table.AddCell(dimensions[i].ToString() + " x " + dimensions[i].ToString());
            table.AddCell("Parallel");
            table.AddCell(resultsOfBenchmarking[i, 1].Item1.ToString());
            table.AddCell(resultsOfBenchmarking[i, 1].Item2.ToString());
        }

        document.Add(table);
        document.Close();
    }

    private static (double, double)[,] MatrixMultiplyBenchmark(int[] dimensions)
    {
        var resultsOfBenchmarking = new (double, double)[AmountOfTestMeasurements, 2];

        for (int i = 0; i < AmountOfTestMeasurements; i++)
        {
            var randomMatrix1 = Matrix.GenerateRandomMatrix(dimensions[i], dimensions[i]);
            var randomMatrix2 = Matrix.GenerateRandomMatrix(dimensions[i], dimensions[i]);

            var timeResultsCommonMultiply = new double[AmountOfMeasurementsForOneDimension];
            var timeResultsParallelMultiply = new double[AmountOfMeasurementsForOneDimension];

            for (var j = 0; j < AmountOfMeasurementsForOneDimension; ++j)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Matrix.Multiply(randomMatrix1, randomMatrix2);
                stopwatch.Stop();
                timeResultsCommonMultiply[j] = stopwatch.ElapsedMilliseconds;
            }

            for (var j = 0; j < AmountOfMeasurementsForOneDimension; ++j)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Matrix.ParallelMultiply(randomMatrix1, randomMatrix2);
                stopwatch.Stop();
                timeResultsParallelMultiply[j] = stopwatch.ElapsedMilliseconds;
            }

            resultsOfBenchmarking[i, 0] = (MathExpectation(timeResultsCommonMultiply), StandardDeviation(timeResultsCommonMultiply));
            resultsOfBenchmarking[i, 1] = (MathExpectation(timeResultsParallelMultiply), StandardDeviation(timeResultsParallelMultiply));
        }

        return resultsOfBenchmarking;
    }

    private static double MathExpectation(double[] times)
        => times.Sum() / times.Length;

    private static double StandardDeviation(double[] times)
    {
        var mathExpectation = MathExpectation(times);
        return Math.Round(Math.Sqrt(times.Sum(t => t * t / times.Length) - mathExpectation * mathExpectation), 1);
    }
}