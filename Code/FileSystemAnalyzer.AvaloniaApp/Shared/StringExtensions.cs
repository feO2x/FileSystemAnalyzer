using System;
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

    public static string FormatSize(this long sizeInBytes)
    {
        sizeInBytes.MustNotBeLessThan(0);
        return sizeInBytes switch
        {
            <= 1024L => $"{sizeInBytes} Bytes",
            <= 1_048_576L => $"{sizeInBytes / 1024.0:N3}KB",
            <= 1_073_741_824L => $"{sizeInBytes / 1_048_576.0:N3}MB",
            <= 1_099_511_627_776L => $"{sizeInBytes / 1_073_741_824.0:N3}GB",
            _ => $"{sizeInBytes / 1_099_511_627_776.0:N3}TB"
        };
    }
}