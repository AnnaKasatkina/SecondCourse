// <copyright file="SingleLinkedList.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SingleLinkedListApp;

/// <summary>
/// Represents a singly linked list of integers.
/// </summary>
public class SingleLinkedList
{
    /// <summary>
    /// Gets or sets the head of the linked list.
    /// </summary>
    public Node Head { get; private set; }

    /// <summary>
    /// Adds a new value to the end of the linked list.
    /// </summary>
    /// <param name="value">Value to be added.</param>
    public void Add(int value)
    {
        if (this.Head == null)
        {
            this.Head = new Node(value);
            return;
        }

        Node current = this.Head;
        while (current.Next != null)
        {
            current = current.Next;
        }

        current.Next = new Node(value);
    }

    /// <summary>
    /// Removes all nodes with odd values from the linked list.
    /// </summary>
    public void RemoveOddValues()
    {
        while (this.Head != null && this.Head.Value % 2 != 0)
        {
            this.Head = this.Head.Next;
        }

        Node current = this.Head;
        while (current?.Next != null)
        {
            if (current.Next.Value % 2 != 0)
            {
                current.Next = current.Next.Next;
            }
            else
            {
                current = current.Next;
            }
        }
    }

    /// <summary>
    /// Converts the linked list to an array.
    /// </summary>
    /// <returns>Array representation of the linked list.</returns>
    public int[] ToArray()
    {
        int count = 0;
        Node current = this.Head;
        while (current != null)
        {
            count++;
            current = current.Next;
        }

        int[] result = new int[count];
        current = this.Head;
        for (int i = 0; i < count; i++)
        {
            result[i] = current.Value;
            current = current.Next;
        }

        return result;
    }
}

/// <summary>
/// Represents a node in a singly linked list.
/// </summary>
public class Node
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Node"/> class.
    /// </summary>
    /// <param name="value">Value of the node.</param>
    public Node(int value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets the value of the node.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Gets or sets the next node in the linked list.
    /// </summary>
    public Node Next { get; set; }
}
