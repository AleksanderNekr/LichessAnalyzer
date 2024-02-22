using System.Text.Json.Serialization;

namespace Backend.DataManagement.Users.Entities;

public class Team(string id, Guid organiserId)
{
    public string Id { get; set; } = id;

    public Guid OrganiserId { get; set; } = organiserId;

    [JsonIgnore]
    public User Organiser { get; set; } = default!;
}
