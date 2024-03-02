using System.Text.Json.Serialization;

namespace Backend.Api.ResponseModels;

public record UserInfoResponse(
    [property: JsonPropertyName("id")]               Guid   Id,
    [property: JsonPropertyName("username")]         string Username,
    [property: JsonPropertyName("email")]            string Email,
    [property: JsonPropertyName("firstName")]        string FirstName,
    [property: JsonPropertyName("lastName")]         string LastName,
    [property: JsonPropertyName("maxPlayersInList")] int    MaxPlayersInList,
    [property: JsonPropertyName("maxListsCount")]    int    MaxListsCount
);
