using System;
using System.Threading.Tasks;
using Bogus;
using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Serilog;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace FileSystemAnalyzer.Tests.AnalysesList;

[UsesVerify]
public sealed class AnalysesListViewModelTests
{
    static AnalysesListViewModelTests()
    {
        VerifySettings = new ();
        VerifySettings.IgnoreMember<AnalysesListViewModel>(viewModel => viewModel.DeleteSelectedAnalysisCommand);
    }
    
    public AnalysesListViewModelTests(ITestOutputHelper output)
    {
        Logger = output.CreateTestLogger();
    }

    private static DateTime ReferenceDate { get; } = new (2022, 11, 5, 9, 35, 30, DateTimeKind.Utc);
    private ILogger Logger { get; }
    
    // TODO: report to Bogus that the filepath in a generated FileNotFoundException does not respect the seed.
    // During my tests, I found out that when Bogus creates a FileNotFoundException with a filePath, the
    // this path is not generated according to the initial seed. Verify picked this up. 
    private Faker<Analysis> AnalysisFaker { get; } = BogusAnalysesSession.CreateFaker(ReferenceDate, false);

    private static VerifySettings VerifySettings { get; }
        

    [Fact]
    public async Task LoadAnalysesAfterInstantiation()
    {
        var viewModel = CreateViewModel();

        await Verifier.Verify(viewModel, VerifySettings);
    }

    [Fact]
    public async Task LoadNextPageOfAnalyses()
    {
        var viewModel = CreateViewModel();
        
        await viewModel.PagingViewModel.LoadNextPageAsync();

        await Verifier.Verify(viewModel, VerifySettings);
    }

    [Fact]
    public async Task SearchMustResetTheAnalysesCollection()
    {
        var viewModel = CreateViewModel();
        await viewModel.PagingViewModel.LoadNextPageAsync();

        viewModel.SearchTerm = "lib";

        await Verifier.Verify(viewModel, VerifySettings);
    }

    [Fact]
    public async Task HasNoAnalyses()
    {
        var viewModel = CreateViewModel(0);

        await Verifier.Verify(viewModel, VerifySettings);
    }

    private AnalysesListViewModel CreateViewModel(int numberOfSessionItems = 300)
    {
        var bogusSession = new BogusAnalysesSession(AnalysisFaker, numberOfSessionItems);
        return new (() => bogusSession, DebouncedValueFactory.NoDebounceFactory, Logger);
    }
}