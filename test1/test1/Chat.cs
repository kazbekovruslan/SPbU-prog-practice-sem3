using System.Net.Sockets;

namespace Chat;

public class Client
{
    private readonly TcpClient client;

    public Client(string ip, int port)
    {
        client = new TcpClient(ip, port);
    }

    public async Task Run()
    {
        Get(client.GetStream());
        Console.WriteLine("Connection is established!");
        await Send(client.GetStream());
    }

    private void Get(NetworkStream stream)
    {
        Task.Run(async () =>
        {
            var reader = new StreamReader(stream);

            var data = await reader.ReadLineAsync();
            while (data != "exit")
            {
                Console.WriteLine(data);
                data = await reader.ReadLineAsync();
            }

            Console.WriteLine("Connection is closed!");
            client.Close();
        });
    }

    private Task Send(NetworkStream stream)
    {
        return Task.Run(async () =>
        {
            var writer = new StreamWriter(stream) { AutoFlush = true };

            var data = Console.ReadLine();
            while (data != "exit")
            {
                await writer.WriteLineAsync(data);
                data = Console.ReadLine();
            }

            Console.WriteLine("Connection is closed!");
            client.Close();
        });
    }
}

public class Server
{
    private readonly TcpListener listener;

    public Server(int port)
    {
        listener = new TcpListener(System.Net.IPAddress.Any, port);
    }

    public async Task Run()
    {
        listener.Start();
        var client = await listener.AcceptTcpClientAsync();
        Console.WriteLine("Connection is established!");
        Get(client.GetStream());
        await Send(client.GetStream());
    }

    private void Get(NetworkStream stream)
    {
        Task.Run(async () =>
                {
                    var reader = new StreamReader(stream);

                    var data = await reader.ReadLineAsync();
                    while (data != "exit")
                    {
                        Console.WriteLine(data);
                        data = await reader.ReadLineAsync();
                    }

                    Console.WriteLine("Connection is closed!");
                    listener.Stop();
                });
    }

    private Task Send(NetworkStream stream)
    {
        return Task.Run(async () =>
                {
                    var writer = new StreamWriter(stream) { AutoFlush = true };

                    var data = Console.ReadLine();
                    while (data != "exit")
                    {
                        await writer.WriteLineAsync(data);
                        data = Console.ReadLine();
                    }

                    Console.WriteLine("Connection is closed!");
                    listener.Stop();
                });
    }
}