using System;
using System.Threading.Tasks;
using Bogus;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.GettingStarted;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Serilog;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace FileSystemAnalyzer.Tests.GettingStarted;

[UsesVerify]
public sealed class ExistingAnalysesViewModelTests
{
    public ExistingAnalysesViewModelTests(ITestOutputHelper output) =>
        Logger = output.CreateTestLogger();

    private static DateTime ReferenceDate { get; } = new (2022, 11, 5, 9, 35, 30, DateTimeKind.Utc);
    private ILogger Logger { get; }
    
    // TODO: report to Bogus that the filepath in a generated FileNotFoundException does not respect the seed.
    // During my tests, I found out that when Bogus creates a FileNotFoundException with a filePath, the
    // this path is not generated according to the initial seed. Verify picked this up. 
    private Faker<Analysis> AnalysisFaker { get; } = BogusAnalysesSession.CreateFaker(ReferenceDate, false);

    [Fact]
    public async Task LoadAnalysesAfterInstantiation()
    {
        var viewModel = CreateViewModel();

        await Verifier.Verify(viewModel);
    }

    [Fact]
    public async Task LoadNextPageOfAnalyses()
    {
        var viewModel = CreateViewModel();
        
        viewModel.LoadAnalyses();

        await Verifier.Verify(viewModel);
    }

    [Fact]
    public async Task SearchMustResetTheAnalysesCollection()
    {
        var viewModel = CreateViewModel();
        viewModel.LoadAnalyses();

        viewModel.SearchTerm = "lib";

        await Verifier.Verify(viewModel);
    }

    [Fact]
    public async Task HasNoAnalyses()
    {
        var viewModel = CreateViewModel(0);

        await Verifier.Verify(viewModel);
    }

    private ExistingAnalysesViewModel CreateViewModel(int numberOfSessionItems = 300)
    {
        var bogusSession = new BogusAnalysesSession(AnalysisFaker, numberOfSessionItems);
        return new (() => bogusSession, DebouncedValueFactory.NoDebounceFactory, Logger);
    }
}