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

            // args.ToList().ForEach(Console.WriteLine);

            switch (args[2])
            {
                case "1":
                    var response = await client.List(path);
                    Console.WriteLine(response);
                    break;
                case "2":
                    var response1 = await client.Get(path);
                    Console.WriteLine($"Size: {response1}\nYour file has been downloaded successfully to a directory 'downloads'!");
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