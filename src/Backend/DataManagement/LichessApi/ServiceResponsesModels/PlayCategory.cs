using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi.ServiceResponsesModels;

public enum PlayCategory
{
    [JsonPropertyName("ultraBullet")]    UltraBullet,
    [JsonPropertyName("bullet")]         Bullet,
    [JsonPropertyName("blitz")]          Blitz,
    [JsonPropertyName("rapid")]          Rapid,
    [JsonPropertyName("classical")]      Classical,
    [JsonPropertyName("correspondence")] Correspondence,
    [JsonPropertyName("chess960")]       Chess960,
    [JsonPropertyName("crazyhouse")]     CrazyHouse,
    [JsonPropertyName("antichess")]      AntiChess,
    [JsonPropertyName("atomic")]         Atomic,
    [JsonPropertyName("horde")]          Horde,
    [JsonPropertyName("kingOfTheHill")]  KingOfTheHill,
    [JsonPropertyName("racingKings")]    RacingKings,
    [JsonPropertyName("threeCheck")]     ThreeCheck
}
