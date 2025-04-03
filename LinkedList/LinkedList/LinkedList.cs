// <copyright file="LinkedList.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LinkedList;

/// <summary>
/// Represents a singly linked list with methods to manipulate the list.
/// </summary>
public class LinkedList
{
    /// <summary>
    /// Gets the head node of the linked list.
    /// </summary>
    public LinkedListNode? Head { get; private set; }

    /// <summary>
    /// Gets the tail node of the linked list.
    /// </summary>
    public LinkedListNode? Tail { get; private set; }

    /// <summary>
    /// Adds a new node with the specified value to the end of the linked list.
    /// </summary>
    /// <param name="value">The value to add to the linked list.</param>
    public void Add(int value)
    {
        var newNode = new LinkedListNode(value);
        if (this.Head == null)
        {
            this.Head = newNode;
            this.Tail = newNode;
        }
        else
        {
            this.Tail!.Next = newNode;
            this.Tail = newNode;
        }
    }

    /// <summary>
    /// Removes consecutive duplicate elements from the linked list.
    /// </summary>
    /// <remarks>
    /// This method modifies the linked list such that no two consecutive nodes have the same value.
    /// </remarks>
    public void RemoveDublicates()
    {
        if (this.Head == null || this.Head.Next == null)
        {
            return;
        }

        LinkedListNode previous = this.Head;
        LinkedListNode? current = previous.Next;

        while (current != null)
        {
            if (current.Value == previous.Value)
            {
                previous.Next = current.Next;
                current = current.Next;

                if (current == null)
                {
                    this.Tail = previous;
                }
            }
            else
            {
                previous = current;
                current = current.Next;
            }
        }
    }

    /// <summary>
    /// Converts the linked list to an array of integers.
    /// </summary>
    /// <returns>An array containing the values of the linked list in order.</returns>
    public int[] ToArray()
    {
        List<int> result = new List<int>();
        LinkedListNode? current = this.Head;
        while (current != null)
        {
            result.Add(current.Value);
            current = current.Next;
        }

        return result.ToArray();
    }
}
