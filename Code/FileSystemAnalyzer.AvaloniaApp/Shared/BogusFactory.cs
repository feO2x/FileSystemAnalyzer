using Bogus;

namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public static class BogusFactory
{
    public static Faker<T> CreateFaker<T>() where T : class => new Faker<T>().UseSeed(42);
}