using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;

namespace Backend.DataManagement.LichessApi.LichessResponsesModels;

public record ActivityResponse(
    [property: JsonPropertyName("interval")] TimeInterval Interval,
    [property: JsonPropertyName("games")]
    [property: JsonConverter(converterType: typeof(GamesDataConverter))] GamesData? GamesData,
    [property: JsonPropertyName("tournaments")] TournamentsData? TournamentsData
);

public record TournamentsData(
    [property: JsonPropertyName("best")] IEnumerable<TournamentStatistic> TournamentStatistics);

public record GamesData(Dictionary<PlayCategory, Stat> Data);

public record Stat(
    [property: JsonPropertyName("win")]  int Wins,
    [property: JsonPropertyName("loss")] int Losses,
    [property: JsonPropertyName("draw")] int Draws);

public record TimeInterval(
    [property: JsonPropertyName("end")] long End);

public class GamesDataConverter : JsonConverter<GamesData>
{
    public override GamesData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Dictionary<PlayCategory, Stat> data = new();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected a property name but got {reader.TokenType}");
            }

            string? propertyName = reader.GetString();

            reader.Read();

            var stat = JsonSerializer.Deserialize<Stat>(ref reader, options);

            if (stat is null || propertyName is null || !Enum.TryParse(propertyName, true, out PlayCategory category))
            {
                continue;
            }

            data.Add(category, stat);
        }

        return new GamesData(data);
    }

    public override void Write(Utf8JsonWriter writer, GamesData value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
