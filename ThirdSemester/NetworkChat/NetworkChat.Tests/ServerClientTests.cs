using System.Net.Sockets;

namespace NetworkChat.Tests
{
    [TestFixture]
    public class ServerClientTests
    {
        [Test]
        public async Task Server_Start_ShouldAcceptClientConnection()
        {
            var port = 5000;
            var serverTask = Task.Run(() => Server.Start(port));

            await Task.Delay(500);
            using var client = new TcpClient();
            Assert.DoesNotThrow(() => client.Connect("127.0.0.1", port));

            serverTask.Dispose();
        }

        [Test]
        public async Task Client_Connect_ShouldConnectToServer()
        {
            var port = 5001;
            var serverTask = Task.Run(() => Server.Start(port));

            await Task.Delay(500);
            Assert.DoesNotThrow(() => Client.Connect("127.0.0.1", port));

            serverTask.Dispose();
        }

        [Test]
        public async Task ClientAndServer_ShouldExchangeMessages()
        {
            var port = 5002;
            var serverTask = Task.Run(() => Server.Start(port));
            await Task.Delay(500);

            using var client = new TcpClient("127.0.0.1", port);
            using var clientStream = client.GetStream();
            using var writer = new StreamWriter(clientStream) { AutoFlush = true };
            using var reader = new StreamReader(clientStream);

            writer.WriteLine("Hello Server");
            var response = reader.ReadLine();

            Assert.AreEqual("Hello Server", response);

            serverTask.Dispose();
        }
    }
}
