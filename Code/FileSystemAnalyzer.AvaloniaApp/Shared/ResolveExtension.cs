using System;
using Avalonia.Markup.Xaml;

namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public sealed class ResolveExtension : MarkupExtension
{
    public Type? Type { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider) =>
        Type is null ? null : App.Current?.Container.GetInstance(Type);
}