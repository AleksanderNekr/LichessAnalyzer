using System.Text.Json.Serialization;

namespace Backend.DataManagement.Users.Entities;

public class AnalyticsList(Guid id, string name, Guid creatorId)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public Guid CreatorId { get; set; } = creatorId;

    [JsonIgnore]
    public User Creator { get; set; } = default!;

    public ICollection<Player>? ListedPlayers { get; set; }
}
