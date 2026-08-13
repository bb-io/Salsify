namespace Apps.Salsify.Helpers.Validation.Models;

public interface IUpdatedDateRangeFilter : IDateFilter
{
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
}