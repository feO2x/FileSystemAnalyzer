using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.GuardClauses;
using Range = Light.GuardClauses.Range;

namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

public sealed class BogusAnalysesSession : IAnalysesSession
{
    public BogusAnalysesSession(Faker<Analysis> faker) => Faker = faker;

    private Faker<Analysis> Faker { get; }
    
    public static Faker<Analysis> CreateFaker()
    {
        var analysisIds = 0;
        return new Faker<Analysis>().RuleFor(analysis => analysis.Id, _ => $"Analyses-{analysisIds++}-A")
                                    .RuleFor(analysis => analysis.DirectoryPath, f => f.System.DirectoryPath())
                                    .RuleFor(analysis => analysis.CreatedAtUtc, f => f.Date.Past().ToUniversalTime())
                                    .RuleFor(analysis => analysis.SizeInBytes, f => f.Random.Number(0, int.MaxValue));
    }


    public void Dispose() { }

    public ValueTask DisposeAsync() => default;
    
    public Task<List<Analysis>> GetAnalysesAsync(int skip, int take, string searchTerm)
    {
        take.MustBeIn(Range.FromInclusive(1).ToInclusive(200));
        var analyses = new List<Analysis>(take);

        for (var i = 0; i < take; i++)
        {
            var fakeAnalysis = Faker.Generate();
            analyses.Add(fakeAnalysis);
        }

        return Task.FromResult(analyses);
    }
}