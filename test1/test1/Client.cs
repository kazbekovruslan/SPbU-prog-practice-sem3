namespace Chat;

using System.Net.Sockets;

/// <summary>
/// The Client class is responsible for establishing a connection to a remote server and managing communication.
/// </summary>
public class Client
{
    private readonly TcpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class with the specified IP address and port number.
    /// </summary>
    /// <param name="ip">The IP address of the remote server.</param>
    /// <param name="port">The port number to connect to on the remote server.</param>
    public Client(string ip, int port)
    {
        client = new TcpClient(ip, port);
    }

    /// <summary>
    /// Starts the client and establishes a connection to the remote server.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
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