// <copyright file="FTPServer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// FTP server that processes List and Get requests to retrieve directory listings
/// and download files. Supports multithreading to handle multiple clients simultaneously.
/// </summary>
public class FTPServer
{
    private readonly int port;
    private TcpListener listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="FTPServer"/> class with the specified port.
    /// </summary>
    /// <param name="port">The port on which the server will listen for incoming connections.</param>
    public FTPServer(int port)
    {
        this.port = port;
        this.listener = new TcpListener(IPAddress.Any, this.port);
    }

    /// <summary>
    /// Starts the server to accept client connections.
    /// </summary>
    public void Start()
    {
        this.listener = new TcpListener(IPAddress.Any, this.port);
        this.listener.Start();
        Console.WriteLine($"Server started on port {this.port}");

        Task.Run(async () => await this.AcceptClientsAsync());
    }

    /// <summary>
    /// Stops the server, ceasing acceptance of new connections.
    /// </summary>
    public void Stop()
    {
        this.listener.Stop();
    }

    private async Task AcceptClientsAsync()
    {
        while (true)
        {
            var client = await this.listener.AcceptTcpClientAsync();
            _ = Task.Run(() => this.HandleClientAsync(client));
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using var networkStream = client.GetStream();
        using var reader = new StreamReader(networkStream, Encoding.UTF8);
        using var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };

        string request = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(request))
        {
            return;
        }

        var parts = request.Split(' ');
        int command = int.Parse(parts[0]);
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
                Console.WriteLine("Invalid command received");
                break;
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
