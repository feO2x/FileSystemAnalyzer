using System.IO;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public readonly record struct DirectoryQueueEntry(DirectoryInfo DirectoryInfo, string? ParentId);