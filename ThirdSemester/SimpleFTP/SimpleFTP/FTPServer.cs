// <copyright file="FtpServer.cs" company="Anna Kasatkina">
// Copyright (c) Anna Kasatkina. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// FTP server that processes List and Get requests to retrieve directory listings
/// and download files. Supports multithreading to handle multiple clients simultaneously.
/// </summary>
public class FtpServer
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly List<Task> clientTasks = new();
    private readonly int port;
    private TcpListener listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="FtpServer"/> class with the specified port.
    /// </summary>
    /// <param name="port">The port on which the server will listen for incoming connections.</param>
    public FtpServer(int port)
    {
        this.port = port;
        this.listener = new TcpListener(IPAddress.Any, this.port);
    }

    /// <summary>
    /// Starts the server to accept client connections.
    /// </summary>
    public void Start()
    {
        this.listener.Start();
        Console.WriteLine($"Server started on port {this.port}");

        Task.Run(async () => await this.AcceptClientsAsync());
    }

    /// <summary>
    /// Stops the server, ceasing acceptance of new connections.
    /// Waits for all client tasks to complete.
    /// </summary>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    public async Task StopAsync()
    {
        this.cancellationTokenSource.Cancel();
        this.listener.Stop();

        Task[] tasks;
        lock (this.clientTasks)
        {
            tasks = this.clientTasks.ToArray();
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("Server stopped.");
    }

    private async Task AcceptClientsAsync()
    {
        while (!this.cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var client = await this.listener.AcceptTcpClientAsync();
                var task = Task.Run(() => this.HandleClientAsync(client), this.cancellationTokenSource.Token);
                lock (this.clientTasks)
                {
                    this.clientTasks.Add(task);
                }

                await task;
                lock (this.clientTasks)
                {
                    this.clientTasks.Remove(task);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            using var networkStream = client.GetStream();
            using var reader = new StreamReader(networkStream, Encoding.UTF8);
            using var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };

            string? request = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(request))
            {
                return;
            }

            var parts = request.Split(' ');
            if (parts.Length < 2 || !int.TryParse(parts[0], out int command))
            {
                return;
            }

            string path = parts[1];
            switch (command)
            {
                case 1:
                    await this.HandleListCommandAsync(path, writer);
                    break;
                case 2:
                    await this.HandleGetCommandAsync(path, writer, networkStream);
                    break;
                default:
                    Console.WriteLine("Invalid command received.");
                    break;
            }
        }
    }

    private async Task HandleListCommandAsync(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }

        var entries = Directory.GetFileSystemEntries(path);
        var response = new StringBuilder();
        response.Append($"{entries.Length}");

        foreach (var entry in entries)
        {
            string name = Path.GetFileName(entry);
            bool isDirectory = Directory.Exists(entry);
            response.Append($" {name} {isDirectory.ToString().ToLower()}");
        }

        await writer.WriteLineAsync(response.ToString());
    }

    private async Task HandleGetCommandAsync(string path, StreamWriter writer, NetworkStream networkStream)
    {
        if (!File.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }

        var fileBytes = await File.ReadAllBytesAsync(path);
        await writer.WriteLineAsync($"{fileBytes.Length}");
        await networkStream.WriteAsync(fileBytes, 0, fileBytes.Length);
    }
}
