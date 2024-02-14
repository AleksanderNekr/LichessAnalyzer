using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi.ServiceResponsesModels;

public enum GameResult
{
    [JsonPropertyName("draw")]      Draw      = 0,
    [JsonPropertyName("whiteWins")] WhiteWins = 1,
    [JsonPropertyName("blackWins")] BlackWins = 2
}
