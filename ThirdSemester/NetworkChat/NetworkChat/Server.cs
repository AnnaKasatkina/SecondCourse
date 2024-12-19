// <copyright file="Server.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NetworkChat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class for working with the server part of the chat.
/// </summary>
public class Server
{
    /// <summary>
    /// Starts the server waiting for clients to connect.
    /// </summary>
    /// <param name="port">The listening port.</param>
    public static void Start(int port)
    {
        Console.WriteLine($"Starting server on port {port}...");
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        try
        {
            using var client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected!");

            using var stream = client.GetStream();
            Task.Run(() => ChatUtils.ReceiveMessages(stream));
            ChatUtils.SendMessages(stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Server error: {ex.Message}");
        }
        finally
        {
            listener.Stop();
            Console.WriteLine("Server shutting down...");
        }
    }
}