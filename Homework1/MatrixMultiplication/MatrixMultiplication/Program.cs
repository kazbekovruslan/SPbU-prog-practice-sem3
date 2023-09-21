namespace MatrixMultiplication;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1 && args.Length != 2)
        {
            Console.WriteLine("Incorrect amount of arguments. Enter 'dotnet run help' to see available arguments.");
        }
        else
        {
            if (args.Length == 1)
            {
                if (args[0] == "help")
                {
                    Console.WriteLine("""

                    This is program that multiplies two matrices commonly and in parallel

                    ---------------------------------------------------------------------

                    Usage:
                    dotnet run 'path to first matrix' 'path to second matrix'
                    
                    ---------------------------------------------------------------------

                    Output files:
                    result.txt - result of multiplication of two matrices commonly
                    result_parallel.txt - result of multiplication of two matrices in parallel

                    """);
                }
                else if (args[0] == "benchmark")
                {
                    Console.WriteLine("Running benchmark...");
                    MatrixBenchmark.WriteResultsOfBenchmarkingToFile();
                    Console.WriteLine("Done!");
                }
            }
            else
            {
                try
                {
                    var firstMatrix = new Matrix(args[0]);
                    var secondMatrix = new Matrix(args[1]);

                    var commonMultiply = Matrix.Multiply(firstMatrix, secondMatrix);
                    commonMultiply.WriteInFile("result.txt");

                    var parallelMultiply = Matrix.ParallelMultiply(firstMatrix, secondMatrix);
                    parallelMultiply.WriteInFile("result_parallel.txt");

                    Console.WriteLine("Done! The result is written to files 'result.txt' and 'result_parallel.txt'.");
                }
                catch (Exception ex) when (ex is ArgumentException ||
                                           ex is FileNotFoundException)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}