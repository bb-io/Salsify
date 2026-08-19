namespace Apps.Salsify.Helpers.LookupTable.Models;

public record TranslatedLookupTable(string SourceAssetId, byte[] Bytes, string FileName)
{
    public string ResolveFileName(string? customName)
    {
        if (string.IsNullOrWhiteSpace(customName))
            return FileName;

        return Path.HasExtension(customName) ? customName : $"{customName}{Path.GetExtension(FileName)}";
    }
}