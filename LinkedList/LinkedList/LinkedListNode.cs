// <copyright file="LinkedListNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LinkedList;

/// <summary>
/// Represents a node in a linked list.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LinkedListNode"/> class with a specified value.
/// </remarks>
/// <param name="value">The value to store in the node.</param>
public class LinkedListNode(int value)
{
    /// <summary>
    /// Gets or sets the value of the node.
    /// </summary>
    public int Value { get; set; } = value;

    /// <summary>
    /// Gets or sets the next node in the linked list.
    /// </summary>
    public LinkedListNode? Next { get; set; } = null;
}
