using System.IO;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public readonly record struct DirectoryStackEntry(DirectoryInfo DirectoryInfo, string? ParentId);