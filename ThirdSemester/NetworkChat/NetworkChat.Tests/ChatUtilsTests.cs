using System.Net.Sockets;

namespace NetworkChat.Tests;

[TestFixture]
public class ChatUtilsTests
{
    [Test]
    public async Task SendMessages_ShouldWriteToNetworkStream()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;

        var clientTask = Task.Run(() =>
        {
            using var client = new TcpClient("127.0.0.1", port);
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            var receivedMessage = reader.ReadLine();
            Assert.AreEqual("Test Message", receivedMessage);
        });

        using var server = await listener.AcceptTcpClientAsync();
        using var serverStream = server.GetStream();

        Console.SetIn(new StringReader("Test Message\nexit"));

        ChatUtils.SendMessages(serverStream);

        listener.Stop();
        await clientTask;
    }

    [Test]
    public async Task ReceiveMessages_ShouldReadFromNetworkStream()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;

        var clientTask = Task.Run(() =>
        {
            using var client = new TcpClient("127.0.0.1", port);
            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            writer.WriteLine("Test Message");
        });

        using var server = await listener.AcceptTcpClientAsync();
        using var serverStream = server.GetStream();

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        Task.Run(() => ChatUtils.ReceiveMessages(serverStream)).Wait(1000);

        Assert.IsTrue(consoleOutput.ToString().Contains("Test Message"));

        listener.Stop();
        await clientTask;
    }
}
