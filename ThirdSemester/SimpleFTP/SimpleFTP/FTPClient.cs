// <copyright file="FtpClient.cs" company="Anna Kasatkina">
// Copyright (c) Anna Kasatkina. All rights reserved.
// </copyright>

using System.Net.Sockets;
using System.Text;

/// <summary>
/// FTP client to send List and Get commands to an FTP server and receive responses.
/// </summary>
public class FtpClient
{
    private readonly string host;
    private readonly int port;

    /// <summary>
    /// Initializes a new instance of the <see cref="FtpClient"/> class with the specified host and port.
    /// </summary>
    /// <param name="host">The server host.</param>
    /// <param name="port">The server port.</param>
    public FtpClient(string host, int port)
    {
        this.host = host;
        this.port = port;
    }

    /// <summary>
    /// Sends the List command to the server to retrieve the list of files and directories.
    /// </summary>
    /// <param name="path">The path to the directory relative to the server’s starting location.</param>
    /// <returns>A list of <see cref="FtpEntry"/> representing files and directories in the specified path.</returns>
    public async Task<List<FtpEntry>> ListAsync(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(this.host, this.port);
        using var networkStream = client.GetStream();
        using var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };
        using var reader = new StreamReader(networkStream, Encoding.UTF8);

        await writer.WriteLineAsync($"1 {path}");
        string? response = await reader.ReadLineAsync();
        if (string.IsNullOrEmpty(response) || response == "-1")
        {
            throw new InvalidOperationException("Failed to retrieve directory listing.");
        }

        var entries = new List<FtpEntry>();
        var parts = response.Split(' ');
        int count = int.Parse(parts[0]);

        if (parts.Length < ((2 * count) + 1))
        {
            throw new InvalidOperationException("Server response format is invalid.");
        }

        for (int i = 0; i < count; i++)
        {
            string name = parts[(2 * i) + 1];
            bool isDirectory = bool.Parse(parts[(2 * i) + 2]);
            entries.Add(new FtpEntry { Name = name, IsDirectory = isDirectory });
        }

        return entries;
    }

    /// <summary>
    /// Sends the Get command to the server to download a file.
    /// </summary>
    /// <param name="path">The path to the file relative to the server’s starting location.</param>
    /// <param name="outputStream">The stream to write the file content to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file is not found on the server.</exception>
    public async Task GetAsync(string path, Stream outputStream)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(this.host, this.port);
        using var networkStream = client.GetStream();
        using var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };
        using var reader = new StreamReader(networkStream, Encoding.UTF8);

        await writer.WriteLineAsync($"2 {path}");
        string? sizeResponse = await reader.ReadLineAsync();
        if (string.IsNullOrEmpty(sizeResponse) || sizeResponse == "-1")
        {
            throw new FileNotFoundException("File not found on server.");
        }

        if (!long.TryParse(sizeResponse, out long size))
        {
            throw new InvalidOperationException("Invalid file size response from server.");
        }

        await networkStream.CopyToAsync(outputStream);
    }
}
