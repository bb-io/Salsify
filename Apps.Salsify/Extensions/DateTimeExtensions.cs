using System.Globalization;

namespace Apps.Salsify.Extensions;

public static class DateTimeExtensions
{
    public static string ToSalsifyStringDate(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
    }
}