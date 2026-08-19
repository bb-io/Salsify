using Apps.Salsify.Helpers.Validation.Models;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Salsify.Helpers.Validation;

public static class ValidatorHelper
{
    public static void ValidateDates(this IDateFilter input)
    {
        List<string> errors = [];

        if (input is IUpdatedDateRangeFilter u &&
            u.UpdatedAfter.HasValue && u.UpdatedBefore.HasValue &&
            u.UpdatedAfter > u.UpdatedBefore)
        {
            errors.Add("'Updated after' date cannot be later than 'Updated before' date");
        }

        if (errors.Count > 0)
            throw new PluginMisconfigurationException(string.Join(". ", errors));
    }
}