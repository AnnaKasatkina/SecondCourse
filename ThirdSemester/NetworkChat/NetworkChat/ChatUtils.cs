// <copyright file="ChatUtils.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NetworkChat;

using System.Net.Sockets;

/// <summary>
/// Utilities for processing chat messages.
/// </summary>
public static class ChatUtils
{
    /// <summary>
    /// Receives messages from the network stream and outputs them to the console.
    /// </summary>
    /// <param name="stream">Network flow.</param>
    public static void ReceiveMessages(NetworkStream stream)
    {
        using var reader = new StreamReader(stream);
        while (true)
        {
            try
            {
                var message = reader.ReadLine();
                if (message == null)
                {
                    break;
                }

                Console.WriteLine($"[Received]: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
                break;
            }
        }
    }

    /// <summary>
    /// Sends messages to the network stream entered by the user.
    /// </summary>
    /// <param name="stream" >Network stream.</param>
    public static void SendMessages(NetworkStream stream)
    {
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        while (true)
        {
            var message = Console.ReadLine() ?? string.Empty;
            if (message.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                break;
            }

            try
            {
                writer.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                break;
            }
        }
    }
}
