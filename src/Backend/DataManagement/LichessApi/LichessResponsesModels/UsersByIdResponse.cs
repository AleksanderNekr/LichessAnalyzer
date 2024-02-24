using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi.LichessResponsesModels;

public record UsersByIdResponse(
    [property: JsonPropertyName("id")]       string                                       Id,
    [property: JsonPropertyName("username")] string                                       Username,
    [property: JsonPropertyName("perfs")]    Dictionary<string, PlayCategoryStatResponse> Performances
);

public record PlayCategoryStatResponse(
    [property: JsonPropertyName("games")]  int GamesCount,
    [property: JsonPropertyName("rating")] int Rating
);