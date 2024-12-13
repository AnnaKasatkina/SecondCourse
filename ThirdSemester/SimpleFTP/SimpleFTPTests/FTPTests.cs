// <copyright file="SimpleFTPTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;

[TestFixture]
public class FTPTests
{
    private const int Port = 12345;
    private FTPServer server;
    private FTPClient client;

    [SetUp]
    public void SetUp()
    {
        this.server = new FTPServer(Port);
        this.server.Start();
        this.client = new FTPClient("localhost", Port);
    }

    [TearDown]
    public void TearDown()
    {
        this.server.Stop();
    }

    [Test]
    public async Task ListDirectory_ShouldReturnDirectoryListing()
    {
        Directory.CreateDirectory("TestDir");
        File.WriteAllText("TestDir/testFile.txt", "Test content");

        string response = await this.client.ListAsync("TestDir");
        Assert.That(response, Does.StartWith("1"));

        Directory.Delete("TestDir", true);
    }

    [Test]
    public async Task GetFile_ShouldReturnFileContent()
    {
        const string filePath = "testFile.txt";
        File.WriteAllText(filePath, "Test content");

        var content = await this.client.GetAsync(filePath);
        Assert.That(content, Is.EqualTo(Encoding.UTF8.GetBytes("Test content")));

        File.Delete(filePath);
    }
}
