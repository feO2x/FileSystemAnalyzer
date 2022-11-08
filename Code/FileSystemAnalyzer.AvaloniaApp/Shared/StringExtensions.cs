using System;
using Humanizer;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public static class StringExtensions
{
    public static string ReplaceSlashesWithSpacesInPath(this string path)
    {
        path.MustNotBeNullOrWhiteSpace();

        Span<char> span = stackalloc char[path.Length];
        for (var i = 0; i < path.Length; i++)
        {
            var character = path[i];
            if (character is '/' or '\\')
                character = ' ';
            span[i] = character;
        }

        return span.ToString();
    }

    public static string NormalizeSearchTerm(this string? searchTerm) =>
        searchTerm.IsNullOrWhiteSpace() ? string.Empty : searchTerm.Trim();

    public static string ConvertToDisplaySize(this long sizeInBytes) =>
        sizeInBytes.Bytes().Humanize("#.##");
}