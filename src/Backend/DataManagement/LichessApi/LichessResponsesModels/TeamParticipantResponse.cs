using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi.LichessResponsesModels;

internal record TeamParticipantResponse(
    [property: JsonPropertyName("username")] string ParticipantName
);
