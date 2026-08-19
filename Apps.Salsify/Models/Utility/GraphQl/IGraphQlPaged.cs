namespace Apps.Salsify.Models.Utility.GraphQl;

public interface IGraphQlPaged<TItem>
{
    GraphQlPage<TItem> Page { get; }
}