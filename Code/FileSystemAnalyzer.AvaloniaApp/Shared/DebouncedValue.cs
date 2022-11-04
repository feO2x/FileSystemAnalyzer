using System;
using System.Collections.Generic;
using Avalonia.Threading;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public sealed class DebouncedValue<T>
{
    public DebouncedValue(T initialValue,
                          Action debouncedValueChanged,
                          int debounceIntervalInMilliseconds = 400,
                          IEqualityComparer<T>? equalityComparer = null,
                          DispatcherPriority priority = DispatcherPriority.Normal)
    {
        debounceIntervalInMilliseconds.MustNotBeLessThan(0);

        CurrentValue = initialValue;
        DebouncedValueChanged = debouncedValueChanged;
        EqualityComparer = equalityComparer;

        if (debounceIntervalInMilliseconds > 0)
            Timer = new (TimeSpan.FromMilliseconds(debounceIntervalInMilliseconds), priority, OnTimerTick);
    }
    
    private Action DebouncedValueChanged { get; }
    private IEqualityComparer<T>? EqualityComparer { get; }
    private DispatcherTimer? Timer { get; }
    public T CurrentValue { get; private set; }

    private void OnTimerTick(object? _, EventArgs __)
    {
        Timer!.Stop();
        DebouncedValueChanged();
    }

    public bool TrySetValue(T value)
    {
        if (EqualityComparer is not null)
        {
            var isDifferent = !EqualityComparer.Equals(CurrentValue, value);
            if (isDifferent)
                SetValueAndUpdateTimer(value);

            return isDifferent;
        }
        else
        {
            var isDifferent = !EqualityComparer<T>.Default.Equals(CurrentValue, value);
            if (!EqualityComparer<T>.Default.Equals(CurrentValue, value))
                SetValueAndUpdateTimer(value);

            return isDifferent;
        }
    }

    private void SetValueAndUpdateTimer(T value)
    {
        CurrentValue = value;
        if (Timer is null)
        {
            DebouncedValueChanged();
            return;
        }

        if (Timer.IsEnabled)
            Timer.Stop();
        Timer.Start();
    }
}