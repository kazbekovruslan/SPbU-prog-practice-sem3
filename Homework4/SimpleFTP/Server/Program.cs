namespace SimpleFTP;

class Program
{
    static void printHelp()
    {
        Console.WriteLine("""
        
        This is the server that supports SimpleFTP protocol.
        Usage: dotnet run {port}

        """);
    }

    static async Task Main(string[] args)
    {
        if (args.Length == 1)
        {
            if (!int.TryParse(args[0], out var port) || port < 0 || port > 65536)
            {
                printHelp();
                return;
            }
            var server = new Server(port);
            await server.Run();
        }
        else
        {
            printHelp();
            return;
        }
    }
}
