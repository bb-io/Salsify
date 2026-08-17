namespace Apps.Salsify.Helpers.Asset.Models;

public record DownloadedAsset(byte[] Bytes, string Filename, string AssetName, string ContentType);