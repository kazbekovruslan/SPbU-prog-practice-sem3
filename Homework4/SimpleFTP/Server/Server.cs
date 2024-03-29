using System.Net.Sockets;
using System.Text;

namespace SimpleFTP;

/// <summary>
/// Represents a server for a simple-FTP protocol connection.
/// </summary>
public class Server
{
    private readonly TcpListener listener;
    private readonly int port;
    private readonly CancellationTokenSource cancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="port">The port number to listen on.</param>
    public Server(int port)
    {
        this.port = port;
        this.listener = new TcpListener(System.Net.IPAddress.Any, port);
        this.cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Starts the server and begins listening for client connections.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Run()
    {
        listener.Start();
        Console.WriteLine("Server started.");

        var tasks = new List<Task>();

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(cancellationTokenSource.Token);

            tasks.Add(Task.Run(
                async () =>
                {
                    using var stream = client.GetStream();
                    using var reader = new StreamReader(stream);
                    using var writer = new StreamWriter(stream) { AutoFlush = true };

                    var data = await reader.ReadLineAsync();
                    if (data != null)
                    {
                        if (data[..2] == "1 ")
                        {
                            await List(data[2..], writer);
                        }
                        if (data[..2] == "2 ")
                        {
                            await Get(data[2..], writer);
                        }
                    }
                }
            ));
        }

        await Task.WhenAll(tasks);
    }

    private static async Task Get(string path, StreamWriter writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteAsync("-1");
            return;
        }
        var content = await File.ReadAllBytesAsync(path);
        await writer.WriteAsync($"{content.Length} {Encoding.UTF8.GetString(content)}");
    }

    private static async Task List(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteAsync("-1");
            return;
        }

        var fileSystemEntries = Directory.GetFileSystemEntries(path);
        var size = fileSystemEntries.Length;

        await writer.WriteAsync($"{size}");
        foreach (var entry in fileSystemEntries)
        {
            await writer.WriteAsync($" {entry} {Directory.Exists(entry)}");
        }
        await writer.WriteAsync("\n");
    }

    /// <summary>
    /// Stops the server and stops listening for client connections.
    /// </summary>
    public void Stop()
    {
        cancellationTokenSource.Cancel();
        listener.Stop();
    }
}