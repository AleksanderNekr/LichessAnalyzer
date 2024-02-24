using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi.LichessResponsesModels;

public record TeamByIdResponse(
    [property: JsonPropertyName("id")]     string Id,
    [property: JsonPropertyName("name")]   string Name,
    [property: JsonPropertyName("leader")] Leader Leader
);

public record Leader(
    [property: JsonPropertyName("name")] string Name
);
