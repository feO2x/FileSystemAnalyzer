using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using FuzzySharp;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

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

    public async Task<List<Analysis>> GetItemsAsync(SearchTermFilters filters,
                                                    int skip,
                                                    int take,
                                                    CancellationToken cancellationToken)
    {
        if (DelayInMilliseconds > 0)
            await Task.Delay(DelayInMilliseconds, cancellationToken).ConfigureAwait(false);

        var searchTerm = filters.SearchTerm;
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

    public void Dispose() { }

    public ValueTask DisposeAsync() => default;

    public async Task RemoveAnalysisAsync(Analysis analysis)
    {
        if (DelayInMilliseconds > 0)
            await Task.Delay(DelayInMilliseconds);

        for (var i = 0; i < Analyses.Count; i++)
        {
            if (!analysis.Equals(Analyses[i]))
                continue;

            Analyses.RemoveAt(i);
            return;
        }

        throw new ArgumentException($"Analysis \"{analysis.Id}\" cannot be deleted because it is not in store");
    }

    public static BogusAnalysesSession Create(int numberOfItems, int delay = 0) =>
        new (CreateFaker(), numberOfItems, delay);

    public static Faker<Analysis> CreateFaker(DateTime? referenceDate = null, bool includeErrorMessages = true)
    {
        var analysisIds = 0;
        var faker = BogusFactory.CreateFaker<Analysis>()
                                .RuleFor(analysis => analysis.Id, _ => $"Analyses-{analysisIds++}-A")
                                .RuleFor(analysis => analysis.DirectoryPath, f => f.System.DirectoryPath())
                                .RuleFor(analysis => analysis.CreatedAtUtc, f => f.Date.Past(refDate: referenceDate).ToUniversalTime())
                                .RuleFor(analysis => analysis.FinishedAtUtc, (f, a) => a.CreatedAtUtc.AddSeconds(f.Random.Double(0.4, 600.0)))
                                .RuleFor(analysis => analysis.SizeInBytes, f => f.Random.Number(0, int.MaxValue))
                                .RuleFor(analysis => analysis.DirectoryPathForSearch, (_, a) => a.DirectoryPath.ReplaceSlashesWithSpacesInPath())
                                .RuleFor(analysis => analysis.NumberOfFiles, f => f.Random.Number(1, 1000))
                                .RuleFor(analysis => analysis.NumberOfFolders, f => f.Random.Number(1, 1000));
        if (includeErrorMessages)
            faker.RuleFor(analysis => analysis.ErrorMessage, f => analysisIds % 20 == 19 ? f.System.Exception().ToString() : null);

        return faker;
    }
}