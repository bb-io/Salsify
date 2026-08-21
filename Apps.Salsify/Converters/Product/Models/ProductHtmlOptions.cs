namespace Apps.Salsify.Converters.Product.Models;

public record ProductHtmlOptions
{
    public required string Locale { get; init; }
    public required string DefaultLocale { get; init; }
    public bool IncludeNonLocalizable { get; init; }
    public IReadOnlyCollection<string> IncludeProperties { get; init; } = [];
    public IReadOnlyCollection<string> ExcludeProperties { get; init; } = [];
}