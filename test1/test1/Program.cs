using System.Net;
using System.Net.Sockets;

namespace Chat;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 1)
        {
            if (!int.TryParse(args[0], out var port))
            {
                Console.WriteLine("Port must be integer number!");
                return;
            }
            var server = new Server(port);

            try
            {
                await server.Run();
            }
            catch (SocketException)
            {
                Console.WriteLine("Socket error!");
            }
            catch (IOException)
            {
                Console.WriteLine("Session is closed!");
            }
        }
        else if (args.Length == 2)
        {
            if (!IPAddress.TryParse(args[0], out var someIp))
            {
                Console.WriteLine("Invalid IP address!");
                return;
            }
            var ip = args[0];
            if (!int.TryParse(args[1], out var port))
            {
                Console.WriteLine("Port must be integer number!");
                return;
            }
            var client = new Client(ip, port);

            try
            {
                await client.Run();
            }
            catch (SocketException)
            {
                Console.WriteLine("Socket error!");
            }
            catch (IOException)
            {
                Console.WriteLine("Session is closed!");
            }
        }
        else
        {
            Console.WriteLine("Invalid amount of arguments!");
            return;
        }
    }
}