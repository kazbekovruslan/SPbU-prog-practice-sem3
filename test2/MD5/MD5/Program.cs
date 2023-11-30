namespace MD5;

class Program
{
    static void PrintHelp()
    {
        Console.WriteLine("""

        This is the application that computes check-sum file system's directory.
        Usage: dotnet run {pathToDirectory}

        """);
    }

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            PrintHelp();
            return;
        }

        try
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var checkSumSingleThread = CheckSum.ComputeCheckSum(args[0]);
            stopwatch.Stop();

            stopwatch.Reset();
            stopwatch.Start();
            var checkSumMultiThread = CheckSum.ComputeCheckSumParallel(args[0]);
            stopwatch.Stop();
            Console.WriteLine($"Single thread: Time - {stopwatch.Elapsed.TotalSeconds}");
            Console.WriteLine($"Multi thread: Time - {stopwatch.Elapsed.TotalSeconds}");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("There is no directory or file on this path!");
            PrintHelp();
            return;
        }
        catch (IOException)
        {
            Console.WriteLine("There is error with reading from file!");
        }
    }
}