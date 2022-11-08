using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public interface IFilesSession : IPagingSession<FileSystemEntry, FilesFilters> { }