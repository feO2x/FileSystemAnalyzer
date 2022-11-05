using System.Collections.Generic;
using System.IO;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Synnotech.Core.Entities;

namespace FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

public sealed class FileSystemEntry : StringEntity
{
    public string FullPath { get; init; } = string.Empty;
    public string FullPathForSearch { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? FileExtension { get; init; }
    public FileSystemEntryType Type { get; init; }
    public long SizeInBytes { get; set; }
    public string AnalysisId { get; init; } = string.Empty;
    public string? ParentId { get; init; }
    public List<string>? ChildIds { get; set; }

    public static FileSystemEntry CreateFolderEntry(string directoryPath,
                                                    string analysisId,
                                                    string? parentId = null)
    {
        directoryPath.MustNotBeNullOrWhiteSpace();
        analysisId.MustNotBeNullOrWhiteSpace();

        return new ()
        {
            FullPath = directoryPath,
            FullPathForSearch = directoryPath.ReplaceSlashesWithSpacesInPath(),
            Name = Path.GetFileName(directoryPath),
            Type = FileSystemEntryType.Folder,
            AnalysisId = analysisId,
            ParentId = parentId
        };
    }

    public static FileSystemEntry FromFileInfo(FileInfo fileInfo,
                                               string analysisId,
                                               string parentId) =>
        CreateFileEntry(fileInfo.FullName, fileInfo.Length, analysisId, parentId);

    public static FileSystemEntry CreateFileEntry(string filePath,
                                                  long sizeInBytes,
                                                  string analysisId,
                                                  string parentId)
    {
        filePath.MustNotBeNullOrWhiteSpace();
        sizeInBytes.MustNotBeLessThan(0L);
        analysisId.MustNotBeNullOrWhiteSpace();
        parentId.MustNotBeNullOrWhiteSpace();

        return new ()
        {
            FullPath = filePath,
            FullPathForSearch = filePath.ReplaceSlashesWithSpacesInPath(),
            Name = Path.GetFileName(filePath),
            FileExtension = Path.GetExtension(filePath),
            Type = FileSystemEntryType.File,
            SizeInBytes = sizeInBytes,
            AnalysisId = analysisId,
            ParentId = parentId
        };
    }

    public void AddChildAndUpdateSize(FileSystemEntry child)
    {
        Check.InvalidOperation(
            Type == FileSystemEntryType.File,
            $"A child entry can only be added to a folder entry, but you tried to add to \"{FullPath}\"."
        );

        ChildIds ??= new ();
        ChildIds.Add(child.Id);
        SizeInBytes += child.SizeInBytes;
    }
}