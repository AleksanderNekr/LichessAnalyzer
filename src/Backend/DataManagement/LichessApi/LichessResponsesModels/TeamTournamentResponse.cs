using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi.LichessResponsesModels;

public record TeamTournamentResponse(
    [property: JsonPropertyName("id")]       string   Id,
    [property: JsonPropertyName("name")]     string   Name,
    [property: JsonPropertyName("startsAt")] DateTime Date
);
