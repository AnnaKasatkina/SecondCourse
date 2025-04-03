// <copyright file="LinkedListTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LinkedList.Tests;

/// <summary>
/// Contains unit tests for the <see cref="LinkedList"/> class.
/// </summary>
[TestFixture]
public class LinkedListTests
{
    /// <summary>
    /// Tests that removing duplicates from an empty list leaves it unchanged.
    /// </summary>
    [Test]
    public void EmptyList()
    {
        var list = new LinkedList();
        list.RemoveDublicates();
        Assert.That(list.Head, Is.Null);
        Assert.That(list.Tail, Is.Null);
    }

    /// <summary>
    /// Tests that removing duplicates from a single-element list leaves it unchanged.
    /// </summary>
    [Test]
    public void SingleElement()
    {
        var list = new LinkedList();
        list.Add(5);
        list.RemoveDublicates();
        Assert.That(list.Head!.Value, Is.EqualTo(5));
        Assert.That(list.Tail!.Value, Is.EqualTo(5));
        Assert.That(list.Head.Next, Is.Null);
    }

    /// <summary>
    /// Tests that consecutive duplicates are correctly removed from the list.
    /// </summary>
    [Test]
    public void MultipleDuplicates()
    {
        var list = new LinkedList();
        int[] input = { 1, 1, 2, 3, 3, 3 };
        foreach (int num in input)
        {
            list.Add(num);
        }

        list.RemoveDublicates();

        int[] expected = { 1, 2, 3 };
        Assert.That(list.ToArray(), Is.EqualTo(expected));
        Assert.That(list.Tail!.Value, Is.EqualTo(3));
    }

    /// <summary>
    /// Tests that when all elements are the same, only one remains after removal.
    /// </summary>
    [Test]
    public void AllElementsSame()
    {
        var list = new LinkedList();
        list.Add(2);
        list.Add(2);
        list.Add(2);

        list.RemoveDublicates();

        Assert.That(list.Head!.Value, Is.EqualTo(2));
        Assert.That(list.Tail!.Value, Is.EqualTo(2));
        Assert.That(list.Head.Next, Is.Null);
    }

    /// <summary>
    /// Tests that a list without duplicates remains unchanged.
    /// </summary>
    [Test]
    public void NoDuplicates()
    {
        var list = new LinkedList();
        int[] input = { 1, 2, 3, 4 };
        foreach (int num in input)
        {
            list.Add(num);
        }

        list.RemoveDublicates();

        Assert.That(list.ToArray(), Is.EqualTo(input));
        Assert.That(list.Tail!.Value, Is.EqualTo(4));
    }

    /// <summary>
    /// Tests that duplicates at the end of the list are correctly handled.
    /// </summary>
    [Test]
    public void DuplicatesAtEnd()
    {
        var list = new LinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(2);
        list.Add(2);

        list.RemoveDublicates();

        Assert.That(list.Tail!.Value, Is.EqualTo(2));
        Assert.That(list.Tail.Next, Is.Null);
        Assert.That(list.Head!.Value, Is.EqualTo(1));
        Assert.That(list.Head.Next!.Value, Is.EqualTo(2));
    }
}