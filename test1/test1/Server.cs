namespace Chat;

using System.Net.Sockets;

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
        using var stream = client.GetStream();
        GetMessage(stream);
        await SendMessage(stream);
    }

    private void GetMessage(NetworkStream stream)
    {
        Task.Run(async () =>
                {
                    using var reader = new StreamReader(stream);
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

    private Task SendMessage(NetworkStream stream)
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
                    await writer.WriteLineAsync(data);

                    Console.WriteLine("Connection is closed!");
                    listener.Stop();
                });
    }
}