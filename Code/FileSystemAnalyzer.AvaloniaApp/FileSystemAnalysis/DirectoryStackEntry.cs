using System.IO;

namespace FileSystemAnalyzer.AvaloniaApp.FileSystemAnalysis;

public readonly record struct DirectoryStackEntry(DirectoryInfo DirectoryInfo, string? ParentId);