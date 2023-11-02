namespace Chat;

class Program
{
    static async Task Main(string[] args)
    {
        if (args[0] == "client")
        {
            var ip = args[1];
            // if (int.TryParse(args[2], out var port))
            // {
            //     Console.WriteLine("Port must be integer number");
            //     return;
            // }
            var port = int.Parse(args[2]);
            var client = new Client(ip, port);
            await client.Run();
        }
        else if (args[0] == "server")
        {
            var port = int.Parse(args[1]);
            var server = new Server(port);
            await server.Run();
        }
    }
}