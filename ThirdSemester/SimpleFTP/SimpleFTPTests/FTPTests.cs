// <copyright file="FtpTests.cs" company="Anna Kasatkina">
// Copyright (c) Anna Kasatkina. All rights reserved.
// </copyright>

using System.Text;

/// <summary>
/// This class contains unit tests for the FTP server and client interactions.
/// It tests FTP functionalities such as listing directories and retrieving files.
/// </summary>
[TestFixture]
public class FtpTests
{
    private const int Port = 12345;
    private FtpServer server;
    private FtpClient client;

    /// <summary>
    /// Sets up the environment for the tests by initializing and starting the FTP server
    /// and creating a client connection to the server.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.server = new FtpServer(Port);
        this.server.Start();
        this.client = new FtpClient("localhost", Port);
    }

    /// <summary>
    /// Cleans up the environment by stopping the FTP server after each test.
    /// </summary>
    /// <returns>A task that represents the asynchronous tear-down operation.</returns>
    [TearDown]
    public async Task TearDownAsync()
    {
        await this.server.StopAsync();
    }

    /// <summary>
    /// Tests the <see cref="FtpClient.ListAsync"/> method to ensure it returns a valid directory listing.
    /// The test creates a directory and a file, then verifies the FTP client can list the directory contents.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task ListDirectory_ShouldReturnDirectoryListing()
    {
        Directory.CreateDirectory("TestDir");
        File.WriteAllText("TestDir/testFile.txt", "Test content");

        var entries = await this.client.ListAsync("TestDir");
        Assert.That(entries.Count, Is.EqualTo(1));
        Assert.That(entries[0].Name, Is.EqualTo("testFile.txt"));
        Assert.That(entries[0].IsDirectory, Is.False);

        Directory.Delete("TestDir", true);
    }

    /// <summary>
    /// Tests the <see cref="FtpClient.GetAsync"/> method to ensure it correctly retrieves the content of a file.
    /// The test creates a file with content, retrieves it using the FTP client, and verifies the retrieved content.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task GetFile_ShouldReturnFileContent()
    {
        const string filePath = "testFile.txt";
        File.WriteAllText(filePath, "Test content");

        using var memoryStream = new MemoryStream();
        await this.client.GetAsync(filePath, memoryStream);

        var content = Encoding.UTF8.GetString(memoryStream.ToArray());
        Assert.That(content, Is.EqualTo("Test content"));

        File.Delete(filePath);
    }
}
