using System;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Synnotech.Core.Entities;

namespace FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

public sealed class Analysis : StringEntity
{
    public string DirectoryPath { get; init; } = string.Empty;
    public string DirectoryPathForSearch { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? FinishedAtUtc { get; set; }
    public string RootEntryId { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public long NumberOfFolders { get; set; }
    public long NumberOfFiles { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static Analysis CreateFromDirectoryPath(string directoryPath, DateTime createdAtUtc)
    {
        directoryPath.MustNotBeNullOrWhiteSpace();
        
        return new ()
        {
            DirectoryPath = directoryPath,
            DirectoryPathForSearch = directoryPath.ReplaceSlashesWithSpacesInPath(),
            CreatedAtUtc = createdAtUtc
        };
    }

    public void SetRootEntry(FileSystemEntry rootEntry)
    {
        rootEntry.Id.MustNotBeNullOrWhiteSpace();
        Check.InvalidArgument(rootEntry.Type == FileSystemEntryType.File, "The root entry of an analysis can only be a folder entry");
        RootEntryId = rootEntry.Id;
        SizeInBytes = rootEntry.SizeInBytes;
    }
}