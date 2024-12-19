// <copyright file="Client.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NetworkChat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class for working with the client part of the chat.
/// </summary>
public static class Client
{
    /// <summary>
    /// Connects to the server at the specified address and port.
    /// </summary>
    /// <param name="ipAddress">The IP address of the server.</param>
    /// <param name="port">The server port.</param>
    public static void Connect(string ipAddress, int port)
    {
        Console.WriteLine($"Connecting to server at {ipAddress}:{port}...");
        using var client = new TcpClient(ipAddress, port);

        Console.WriteLine("Connected to server!");

        using var stream = client.GetStream();
        Task.Run(() => ChatUtils.ReceiveMessages(stream));
        ChatUtils.SendMessages(stream);

        Console.WriteLine("Client shutting down...");
    }
}
