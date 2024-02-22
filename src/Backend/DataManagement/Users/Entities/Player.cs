using System.Text.Json.Serialization;

namespace Backend.DataManagement.Users.Entities;

public class Player(string id, Guid containingListId)
{
    public string Id { get; set; } = id;

    [JsonIgnore]
    public AnalyticsList ContainingList { get; set; } = default!;

    public Guid ContainingListId { get; set; } = containingListId;
}
