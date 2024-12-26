// <copyright file="FtpEntry.cs" company="Anna Kasatkina">
// Copyright (c) Anna Kasatkina. All rights reserved.
// </copyright>

/// <summary>
/// Represents an entry in the directory listing on the FTP server.
/// </summary>
public class FtpEntry
{
    /// <summary>
    /// Gets or sets the name of the file or directory.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this entry is a directory.
    /// </summary>
    public bool IsDirectory { get; set; }
}
