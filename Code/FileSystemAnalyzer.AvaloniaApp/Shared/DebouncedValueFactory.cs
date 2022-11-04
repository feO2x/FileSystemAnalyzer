using System;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public sealed class DebouncedValueFactory
{
    public static DebouncedValueFactory NoDebounceFactory { get; } = new (0);
    public static DebouncedValueFactory DefaultFactory { get; } = new (400);
    
    public DebouncedValueFactory(int debounceIntervalInMilliseconds) =>
        DebounceIntervalInMilliseconds = debounceIntervalInMilliseconds.MustNotBeLessThan(0);

    private int DebounceIntervalInMilliseconds { get; }
    
    public DebouncedValue<T> CreateDebouncedValue<T>(T initialValue, Action debouncedValueChanged) =>
        new (initialValue, debouncedValueChanged, DebounceIntervalInMilliseconds);
}