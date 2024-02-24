using System.Text.Json.Serialization;
using Backend.DataManagement.LichessApi.LichessResponsesModels;

namespace Backend.DataManagement.LichessApi.ServiceResponsesModels;

public class TeamResponse(
    string                                id,
    string                                name,
    string                                organiserNickname,
    IReadOnlyList<string>                 participants,
    IReadOnlyList<TeamTournamentResponse> tournaments
)
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = id;

    [JsonPropertyName("name")]
    public string Name { get; init; } = name;

    [JsonPropertyName("organiserNickname")]
    public string OrganiserNickname { get; init; } = organiserNickname;

    [JsonPropertyName("participants")]
    public IReadOnlyList<string> Participants { get; set; } = participants;

    [JsonPropertyName("tournaments")]
    public IReadOnlyList<TeamTournamentResponse> Tournaments { get; set; } = tournaments;
}
