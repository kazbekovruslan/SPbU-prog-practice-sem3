using System.Net.Sockets;
using System.Text;

namespace SimpleFTP;

public class Client
{
    private readonly int port;
    private readonly string ip;

    public Client(string ip, int port)
    {
        this.port = port;
        this.ip = ip;
    }

    public async Task<string?> List(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip, port);

        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);

        await writer.WriteLineAsync($"1 {path}");
        // var response = await reader.ReadLineAsync();
        // ReadLineAsync нельзя, потому что в сообщении могут быть \n
        var response = await reader.ReadToEndAsync();

        return response;
    }

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
        var content = responseArray[1];

        await File.WriteAllBytesAsync($"{path}", Encoding.UTF8.GetBytes(content));

        return size;
    }
}