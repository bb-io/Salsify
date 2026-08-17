using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Filters.Bilingual.Xliff2;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;
using ClosedXML.Excel;

namespace Apps.Salsify.Extensions;

public static class StreamExtensions
{
    public static async Task<string> ToHtmlString(this Stream fileStream, string fileName)
    {
        var bytes = await fileStream.GetByteData();

        if (!Xliff2Serializer.IsXliff2(new MemoryStream(bytes), out _))
            return bytes.ToUtf8String();

        var loaded = Transformation.Load(new MemoryStream(bytes), fileName);
        if (!loaded.Success)
            throw new PluginMisconfigurationException(loaded.Error);

        var target = loaded.Value.Target();
        if (!target.Success)
            throw new PluginMisconfigurationException(target.Error);

        return target.Value.ToStream().ReadString();
    }
    
    public static XLWorkbook ToWorkbook(this Stream stream)
    {
        try
        {
            return new XLWorkbook(stream);
        }
        catch
        {
            throw new PluginMisconfigurationException("Asset could not be read as a spreadsheet. Only .xlsx files are supported");
        }
    }
}