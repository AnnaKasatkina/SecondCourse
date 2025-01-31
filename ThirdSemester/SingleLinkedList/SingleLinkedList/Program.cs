// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SingleLinkedListApp;

internal class Program
{
    private static void Main()
    {
        SingleLinkedList list = new SingleLinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Add(5);

        Console.WriteLine("Original List: " + string.Join(", ", list.ToArray()));

        list.RemoveOddValues();

        Console.WriteLine("List after removing odd values: " + string.Join(", ", list.ToArray()));
    }
}
