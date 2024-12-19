using System;
using System.Net.Sockets;
using System.Threading.Tasks;
namespace NetworkChat.Tests
{
    /// <summary>
    /// Unit tests for server and client functionality.
    /// </summary>
    [TestFixture]
    public class ServerClientTests
    {
        /// <summary>
        /// Verifies that the client successfully connects to the server.
        /// </summary>
        [Test]
        public async Task Client_Connect_ShouldConnectToServer()
        {
            var port = 5001;
            var serverTask = Task.Run(() => Server.Start(port));

            await Task.Delay(500);
            using var client = new TcpClient();
            Assert.DoesNotThrow(() => client.Connect("127.0.0.1", port));

            serverTask.Dispose();
        }

        /// <summary>
        /// Verifies message exchange between client and server.
        /// </summary>
        [Test]
        public async Task ClientAndServer_ShouldExchangeMessages()
        {
            var port = 5002;
            var listener = new TcpListener(System.Net.IPAddress.Loopback, port);
            listener.Start();

            var serverTask = Task.Run(async () =>
            {
                using var server = await listener.AcceptTcpClientAsync();
                using var stream = server.GetStream();
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream) { AutoFlush = true };

                var message = await reader.ReadLineAsync();
                await writer.WriteLineAsync(message);
            });

            await Task.Delay(500);
            using var client = new TcpClient("127.0.0.1", port);
            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            using var reader = new StreamReader(stream);

            writer.WriteLine("Hello Server");
            var response = await reader.ReadLineAsync();

            Assert.Equals("Hello Server", response);

            listener.Stop();
            await serverTask;
        }
    }
}
