namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public interface IConverter<in TSource, out TTarget>
{
    TTarget Convert(TSource value);
}