using Apps.Salsify.Extensions;

namespace Apps.Salsify.Helpers.Query;

public sealed record Filter
{
    public string Clause { get; }
    
    private Filter(string clause) => Clause = clause;

    public static Filter? EqualTo(string? field, string? value)
    {
        return IsIncomplete(field, value) 
            ? null 
            : new($"'{field}':'{value}'");
    }
    
    public static Filter? Contains(string? field, string? value)
    {
        return IsIncomplete(field, value) 
            ? null 
            : new($"'{field}':contains('{value}')");
    }

    public static Filter? GreaterOrEqual(string? field, DateTime? value)
    {
        return IsIncomplete(field, value) 
            ? null 
            : new($"'{field}':gte('{value!.Value.ToSalsifyStringDate()}')");
    }

    public static Filter? LessOrEqual(string? field, DateTime? value)
    {
        return IsIncomplete(field, value) 
            ? null 
            : new($"'{field}':lte('{value!.Value.ToSalsifyStringDate()}')");
    }

    public static Filter? Raw(string? rawQuery)
    {
        return string.IsNullOrWhiteSpace(rawQuery) 
            ? null 
            : new(rawQuery.TrimStart('='));
    }

    public static Filter? InList(string? listId)
    {
        return string.IsNullOrWhiteSpace(listId) 
            ? null 
            : new($"list:{listId}");
    }

    public static string Build(params Filter?[] clauses)
    {
        return $"={string.Join(",", clauses.Where(x => x is not null).Select(x => x!.Clause))}";
    }
    
    private static bool IsIncomplete(string? field, string? value)
    {
        return string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(value);
    }

    private static bool IsIncomplete(string? field, DateTime? value)
        => string.IsNullOrWhiteSpace(field) || value is null;
}