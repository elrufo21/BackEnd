using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ecommerce.Api.Converters;

/// <summary>
/// Allows empty strings to bind as null and enforces ISO date parsing (yyyy-MM-dd).
/// </summary>
public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            // Accept plain date (yyyy-MM-dd)
            if (DateTime.TryParseExact(str, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            // Accept ISO with time (e.g., 1996-05-21T00:00:00 or with Z)
            if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out date))
            {
                return date;
            }
        }

        throw new JsonException($"Invalid date format. Expected {DateFormat} or null.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
