using Apps.Salsify.Models.Entities.Asset;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Responses.Asset;

public class AssetResponse(AssetEntity entity)
{
    [Display("Asset ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Asset name")]
    public string? Name { get; set; } = entity.Name;

    [Display("Asset created at")]
    public DateTime CreatedAt { get; set; } = entity.CreatedAt;

    [Display("Asset updated at")]
    public DateTime UpdatedAt { get; set; } = entity.UpdatedAt;

    [Display("Asset status")]
    public string Status { get; set; } = entity.Status;

    [Display("Asset resource type")]
    public string? ResourceType { get; set; } = entity.ResourceType;

    [Display("Asset filename")]
    public string? Filename { get; set; } = entity.Filename;

    [Display("Asset bytes size")]
    public int Bytes { get; set; } = entity.Bytes;

    [Display("Asset format")]
    public string? Format { get; set; } = entity.Format;
}