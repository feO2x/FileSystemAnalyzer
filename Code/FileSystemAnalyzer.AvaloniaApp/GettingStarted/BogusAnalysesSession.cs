using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using FuzzySharp;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by the DI container
public sealed class BogusAnalysesSession : IAnalysesSession
{
    public BogusAnalysesSession(Faker<Analysis> faker, int numberOfItems = 300, int delayInMilliseconds = 0)
    {
        DelayInMilliseconds = delayInMilliseconds.MustNotBeLessThan(0);
        numberOfItems.MustNotBeLessThan(0);

        Analyses = new (numberOfItems);
        for (var i = 0; i < numberOfItems; i++)
        {
            Analyses.Add(faker.Generate());
        }

        // Sort by CreatedAtUtc descending
        Analyses.Sort((x, y) => y.CreatedAtUtc.CompareTo(x.CreatedAtUtc));
    }

    private int DelayInMilliseconds { get; }
    private List<Analysis> Analyses { get; }

    public void Dispose() { }

    public ValueTask DisposeAsync() => default;

    public static BogusAnalysesSession Create(int numberOfItems, int delay = 0) =>
        new (CreateFaker(), numberOfItems, delay);

    public async Task<List<Analysis>> GetAnalysesAsync(int skip, int take, string searchTerm, CancellationToken cancellationToken)
    {
        if (DelayInMilliseconds > 0)
            await Task.Delay(DelayInMilliseconds, cancellationToken).ConfigureAwait(false);

        List<Analysis> result;
        if (searchTerm.IsNullOrWhiteSpace())
        {
            result = Analyses.Skip(skip)
                             .Take(take)
                             .ToList();
            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }

        var prototype = new Analysis { DirectoryPathForSearch = searchTerm };
        result = Process.ExtractSorted(prototype,
                                       Analyses,
                                       analysis => analysis.DirectoryPathForSearch,
                                       cutoff: 50)
                        .Select(extractionResult => extractionResult.Value)
                        .Skip(skip)
                        .Take(take)
                        .ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return result;
    }

    public static Faker<Analysis> CreateFaker()
    {
        var analysisIds = 0;
        return new Faker<Analysis>().RuleFor(analysis => analysis.Id, _ => $"Analyses-{analysisIds++}-A")
                                    .RuleFor(analysis => analysis.DirectoryPath, f => f.System.DirectoryPath())
                                    .RuleFor(analysis => analysis.CreatedAtUtc, f => f.Date.Past().ToUniversalTime())
                                    .RuleFor(analysis => analysis.SizeInBytes, f => f.Random.Number(0, int.MaxValue))
                                    .RuleFor(analysis => analysis.DirectoryPathForSearch, (_, a) => a.DirectoryPath.ReplaceSlashesWithSpacesInPath());
    }
}