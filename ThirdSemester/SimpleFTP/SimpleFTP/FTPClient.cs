// <copyright file="FTPClient.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net.Sockets;
using System.Text;

/// <summary>
/// FTP client to send List and Get commands to an FTP server and receive responses.
/// </summary>
public class FTPClient
{
    private readonly string host;
    private readonly int port;

    /// <summary>
    /// Initializes a new instance of the <see cref="FTPClient"/> class with the specified host and port.
    /// </summary>
    /// <param name="host">The server host.</param>
    /// <param name="port">The server port.</param>
    public FTPClient(string host, int port)
    {
        this.host = host;
        this.port = port;
    }

    /// <summary>
    /// Sends the List command to the server to retrieve the list of files and directories.
    /// </summary>
    /// <param name="path">The path to the directory relative to the server’s starting location.</param>
    /// <returns>The server's response containing the number and names of files and directories.</returns>
    public async Task<string> ListAsync(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(this.host, this.port);
        using var networkStream = client.GetStream();
        using var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };
        using var reader = new StreamReader(networkStream, Encoding.UTF8);

        await writer.WriteLineAsync($"1 {path}");
        return await reader.ReadLineAsync();
    }

    /// <summary>
    /// Sends the Get command to the server to download a file.
    /// </summary>
    /// <param name="path">The path to the file relative to the server’s starting location.</param>
    /// <returns>A byte array containing the file’s content.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file is not found on the server.</exception>
    public async Task<byte[]> GetAsync(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(this.host, this.port);
        using var networkStream = client.GetStream();
        using var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };
        using var reader = new StreamReader(networkStream, Encoding.UTF8);

        await writer.WriteLineAsync($"2 {path}");
        string sizeResponse = await reader.ReadLineAsync();

        if (long.TryParse(sizeResponse, out long size) && size == -1)
        {
            throw new FileNotFoundException("File not found on server.");
        }

        var buffer = new byte[size];
        await networkStream.ReadAsync(buffer, 0, buffer.Length);
        return buffer;
    }
}
