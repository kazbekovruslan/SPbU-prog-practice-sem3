using System.Net;

namespace SimpleFTP;

class Program
{
    static void printHelp()
    {
        Console.WriteLine("""
        
        This is the client that support SimpleFTP protocol.

        There are 2 requests:
        List - "1 <path: String>\n"
        Get - "2 <path: String>\n"

        Usage: dotnet run {ip} {port} {command} {path}

        """);
    }

    static async Task Main(string[] args)
    {
        if (args.Length == 4)
        {
            if (!IPAddress.TryParse(args[0], out _) || !int.TryParse(args[1], out var port)
            || port < 0 || port > 65536)
            {
                printHelp();
                return;
            }

            var ip = args[0];
            var client = new Client(ip, port);
            var path = args[3];

            switch (args[2])
            {
                case "1":
                    var responseFromList = await client.List(path);
                    Console.WriteLine(responseFromList);
                    break;
                case "2":
                    var responseFromGet = await client.Get(path);
                    if (responseFromGet == "-1")
                    {
                        Console.WriteLine("File doesn't exist!");
                        return;
                    }
                    Console.WriteLine($"Size: {responseFromGet}\nYour file has been downloaded successfully!");
                    break;
                default:
                    printHelp();
                    return;
            }
        }
        else
        {
            printHelp();
            return;
        }
    }
}