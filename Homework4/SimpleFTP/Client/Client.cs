using System.Net.Sockets;
using System.Text;

namespace SimpleFTP;

/// <summary>
/// Represents a client for a simple-FTP protocol connection.
/// </summary>
public class Client
{
    private readonly int port;
    private readonly string ip;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="ip">The IP address of the server.</param>
    /// <param name="port">The port number of the server.</param>
    public Client(string ip, int port)
    {
        this.port = port;
        this.ip = ip;
    }

    /// <summary>
    /// Lists the files at the specified path on the FTP server.
    /// </summary>
    /// <param name="path">The path on the server to list files from.</param>
    /// <returns>A string representing the response from the server.</returns>
    public async Task<string?> List(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip, port);

        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);

        await writer.WriteLineAsync($"1 {path}");
        var response = await reader.ReadToEndAsync();

        return response;
    }

    /// <summary>
    /// Retrieves a file from the specified path on the server.
    /// </summary>
    /// <param name="path">The path on the server to retrieve the file from.</param>
    /// <returns>A string representing the size of the file retrieved from the server.</returns>
    public async Task<string?> Get(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip, port);

        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);

        await writer.WriteLineAsync($"2 {path}");
        var response = await reader.ReadToEndAsync();

        var responseArray = response.Split(" ", 2);
        var size = responseArray[0];

        if (responseArray.Length > 1)
        {
            var content = responseArray[1];
            Directory.CreateDirectory("downloads");
            var nameOfFile = path.Split("/").Last();
            await File.WriteAllBytesAsync($"downloads/{nameOfFile}", Encoding.UTF8.GetBytes(content));
        }

        return size;
    }
}