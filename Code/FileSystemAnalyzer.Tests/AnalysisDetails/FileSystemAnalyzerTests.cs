using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Serilog;
using Serilog.Events;
using Synnotech.Core.Entities;
using Synnotech.DatabaseAbstractions.Mocks;
using Synnotech.Time;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Analyzer = FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.FileSystemAnalyzer;

namespace FileSystemAnalyzer.Tests.AnalysisDetails;

[UsesVerify]
public sealed class FileSystemAnalyzerTests
{
    public FileSystemAnalyzerTests(ITestOutputHelper output)
    {
        Session = new ();
        var clock = new TestClock(new DateTime(2022, 11, 6, 11, 43, 42, DateTimeKind.Utc));
        var logger = output.CreateTestLogger(levelSwitch: new (LogEventLevel.Debug));
        Progress = new (logger);
        Analyzer = new (() => Session, clock, logger);
    }

    private AnalysisSessionMock Session { get; }
    private ProgressSpy Progress { get; }
    private Analyzer Analyzer { get; }

    [Fact]
    public async Task AnalyzeSampleFolderStructure()
    {
        var targetDirectory = SetupDirectory();

        var analysis = await Analyzer.CreateAnalysisAsync(targetDirectory);
        await Analyzer.AnalyzeFileSystemAsync(analysis, Progress);

        var verifySettings = new VerifySettings();
        verifySettings.IgnoreMember<FileSystemEntry>(e => e.FullPathForSearch);
        verifySettings.IgnoreMember<Analysis>(a => a.DirectoryPathForSearch);
        await Verifier.Verify(Session, verifySettings);
    }

    private static string SetupDirectory()
    {
        /* This method creates the following structure in the local file system (value in parentheses is size in Bytes):
         *
         * - A (11,334)
         *   - B (1029)
         *     - e.bin (1024)
         *     - f.txt (5)
         *   - C (10,265)
         *     - I (2060)
         *       - j.txt (12)
         *       - k.bin (2048)
         *     - g.ini (13)
         *     - h.bin (8192)
         *   - d.json (40)
         */

        var parentDirectory = Path.GetDirectoryName(typeof(FileSystemAnalyzerTests).Assembly.Location)!;

        var random = new Random(42);
        var buffer = new byte[8_192];
        var a = Path.Combine(parentDirectory, "A");
        var b = Path.Combine(a, "B");
        var c = Path.Combine(a, "C");
        var d = Path.Combine(a, "d.json");
        var e = Path.Combine(b, "e.bin");
        var f = Path.Combine(b, "f.txt");
        var g = Path.Combine(c, "g.ini");
        var h = Path.Combine(c, "h.bin");
        var i = Path.Combine(c, "I");
        var j = Path.Combine(i, "j.txt");
        var k = Path.Combine(i, "k.bin");

        if (Directory.Exists(a))
            Directory.Delete(a, true);

        Directory.CreateDirectory(b);
        Directory.CreateDirectory(i);

        File.WriteAllText(d, "{ \"value\": \"This is a small JSON file\" }");

        var oneKbSpan = new Span<byte>(buffer, 0, 1024);
        random.NextBytes(oneKbSpan);
        using var stream1 = new FileStream(e, FileMode.Create);
        stream1.Write(oneKbSpan);

        File.WriteAllText(f, "Hello");

        File.WriteAllText(g, "wait = for it");

        random.NextBytes(buffer);
        using var stream2 = new FileStream(h, FileMode.Create);
        stream2.Write(buffer);

        File.WriteAllText(j, "So deep down");

        var twoKbSpan = new Span<byte>(buffer, 0, 2048);
        random.NextBytes(twoKbSpan);
        using var stream3 = new FileStream(k, FileMode.Create);
        stream3.Write(twoKbSpan);

        return a;
    }

    private sealed class AnalysisSessionMock : AsyncSessionMock, IFileSystemAnalysisSession
    {
        // ReSharper disable MemberCanBePrivate.Local -- these properties need to be public because of Verify serialization
        public List<Analysis> CapturedAnalyses { get; } = new ();

        public List<FileSystemEntry> CapturedEntries { get; } = new ();
        // ReSharper restore MemberCanBePrivate.Local

        public Task StoreAsync<T>(T entity, CancellationToken cancellationToken)
        {
            switch (entity)
            {
                case Analysis analysis:
                    // The session is instantiated several times during analysis,
                    // but for this test, the session is a singleton. With several
                    // sessions, the analysis is attached to each session if needed.
                    // We solve this problem in the tests by simply adding the same
                    // analysis instance once.
                    for (var i = 0; i < CapturedAnalyses.Count; i++)
                    {
                        if (ReferenceEquals(analysis, CapturedAnalyses[i]))
                            return Task.CompletedTask;
                    }

                    CapturedAnalyses.Add(analysis);
                    analysis.ToMutable().SetId($"Analysis-{CapturedAnalyses.Count}-A");
                    break;
                case FileSystemEntry fileSystemEntry:
                    CapturedEntries.Add(fileSystemEntry);
                    fileSystemEntry.ToMutable().SetId($"FileSystemEntry-{CapturedEntries.Count}-A");
                    break;
                default:
                    throw new ArgumentException($"Cannot process entity type \"{typeof(T)}\".", nameof(entity));
            }

            return Task.CompletedTask;
        }

        public Task<FileSystemEntry> GetFileSystemEntryAsync(string id, CancellationToken cancellationToken)
        {
            var entries = CapturedEntries;
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry.Id == id)
                    return Task.FromResult(entry);
            }

            throw new ArgumentException($"Cannot find entry with ID \"{id}\".", nameof(id));
        }

        public void EvictFileSystemEntries(List<FileSystemEntry> entries) { }
    }

    private sealed class ProgressSpy : IProgress<ProgressState>
    {
        public ProgressSpy(ILogger logger)
        {
            Logger = logger;
        }

        private ILogger Logger { get; }

        public void Report(ProgressState value) =>
            Logger.Information("{@ProgressState}", value);
    }
}