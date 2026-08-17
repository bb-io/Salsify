using System.Text;

namespace Apps.Salsify.Extensions;

public static class StringExtensions
{
    public static string RemoveFromEnd(this string source, params string[] toRemove)
    {
        if (string.IsNullOrWhiteSpace(source)) 
            return source;

        foreach (string suffix in toRemove)
        {
            if (!source.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) 
                continue;

            return source[..^suffix.Length];
        }

        return source;
    }
    
    public static string ToUtf8String(this byte[] bytes) => Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF');
}