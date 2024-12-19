// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using NetworkChat;

if (args.Length == 1 && int.TryParse(args[0], out var port))
{
    try
    {
        Server.Start(port);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error starting server: {ex.Message}");
    }
}
else if (args.Length == 2 && int.TryParse(args[1], out port))
{
    try
    {
        Client.Connect(args[0], port);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error connecting to server: {ex.Message}");
    }
}
else
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  As server: NetworkChat <port>");
    Console.WriteLine("  As client: NetworkChat <IP> <port>");
}