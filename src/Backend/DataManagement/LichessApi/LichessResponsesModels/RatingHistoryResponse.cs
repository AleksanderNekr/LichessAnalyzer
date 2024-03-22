using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi.LichessResponsesModels;

public record RatingHistoryResponse(
    [property: JsonPropertyName("name")]   string             Category,
    [property: JsonPropertyName("points")] IEnumerable<int[]> PointsPerDate
);
