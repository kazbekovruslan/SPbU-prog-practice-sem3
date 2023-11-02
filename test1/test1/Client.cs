namespace Chat;

using System.Net.Sockets;

public class Client
{
    private readonly TcpClient client;

    public Client(string ip, int port)
    {
        client = new TcpClient(ip, port);
    }

    public async Task Run()
    {
        using var stream = client.GetStream();
        GetMessage(stream);
        Console.WriteLine("Connection is established!");
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
            client.Close();
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
            client.Close();
        });
    }
}