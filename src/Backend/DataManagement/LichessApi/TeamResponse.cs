using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi;

public record TeamResponse(
    [property: JsonPropertyName("id")]                string                Id,
    [property: JsonPropertyName("name")]              string                Name,
    [property: JsonPropertyName("organiserNickname")] string                OrganiserNickname,
    [property: JsonPropertyName("participants")]      IReadOnlyList<string> Participants,
    [property: JsonPropertyName("tournaments")]       IReadOnlyList<string> Tournaments
);
