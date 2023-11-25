using System.ComponentModel;
using System.Dynamic;
using System.Net.Sockets;

namespace SimpleFTP;

public class Server
{
    private TcpListener listener;
    private int port;
    private CancellationTokenSource cancellationTokenSource;

    public Server(int port)
    {
        this.port = port;
        this.listener = new TcpListener(System.Net.IPAddress.Any, port);
        this.cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task Run()
    {
        listener.Start();
        Console.WriteLine("Server started.");

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(cancellationTokenSource.Token);
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
    }

    private static async Task Get(string path, StreamWriter writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }

        var content = File.OpenRead(path);
        var size = new FileInfo(path).Length;
        await writer.WriteAsync($"{size} ");
        await content.CopyToAsync(writer.BaseStream);
        await writer.WriteLineAsync();
    }

    private static async Task List(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
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
}