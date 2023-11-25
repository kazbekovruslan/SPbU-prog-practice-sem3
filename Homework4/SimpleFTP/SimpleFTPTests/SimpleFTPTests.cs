namespace SimpleFTP;

public class Tests
{
    private Client client;
    private Server server;

    [OneTimeSetUp]
    public void Init()
    {
        Directory.CreateDirectory("files");
        File.WriteAllText("files/test.txt", "1234567");
        server = new Server(1234);
        client = new Client("127.0.0.1", 1234);
        _ = Task.Run(() => server.Run());
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        server.Stop();
        Directory.Delete("files", true);
        Directory.Delete("downloads", true);
        Task.Delay(500);
    }

    [Test]
    public async Task ListShouldReturnRightAnswer()
    {
        var actual = await client.List("files");
        var expected = "1 files\\test.txt False\n";

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetShouldReturnRightAnswer()
    {
        var response = await client.Get("files/test.txt");

        var expected = File.ReadAllBytes("files/test.txt");
        var actual = File.ReadAllBytes("downloads/test.txt");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetForNonExistingFileShouldReturnMinusOne()
    {
        var actual = await client.Get("nonExistingFile.txt");
        var expected = "-1";

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task ListForNonExistingDirectoryShouldReturnMinusOne()
    {
        var actual = await client.List("nonExistingDirectory");
        var expected = "-1";

        Assert.That(actual, Is.EqualTo(expected));
    }
}