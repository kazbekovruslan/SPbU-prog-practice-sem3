namespace MatrixMultiplication;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Incorrect amount of arguments. Enter 'dotnet run [path to 1 matrix] [path to 2 matrix]'.");
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