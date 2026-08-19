using Apps.Salsify.Api;
using Apps.Salsify.Constants;
using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;

namespace Apps.Salsify.Helpers;

public static class PropertyHelper
{
    private const int ReportBatchSize = 100;

    // https://developers.salsify.com/reference/read-multiple-properties-report
    public static async Task<Dictionary<string, PropertyEntity>> GetDefinitions(SalsifyClient client, IEnumerable<string> propertyIds)
    {
        var ids = propertyIds.Distinct(StringComparer.Ordinal).ToArray();
        var definitions = new Dictionary<string, PropertyEntity>(StringComparer.Ordinal);

        foreach (var batch in ids.Chunk(ReportBatchSize))
        {
            var request = new SalsifyRequest("properties") { OverrideVerb = CustomHttpVerbs.Report }
                .WithJsonBody(new { ids = batch });

            var entities = await client.ExecuteWithErrorHandling<List<PropertyEntity?>>(request);

            foreach (var entity in entities.Where(x => x is not null))
                definitions[entity!.Id] = entity;
        }

        return definitions;
    }
}