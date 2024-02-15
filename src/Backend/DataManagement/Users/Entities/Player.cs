namespace Backend.DataManagement.Users.Entities;

public class Player(string id, Guid containingListId)
{
    public string Id { get; set; } = id;

    public AnalyticsList ContainingList { get; set; } = default!;

    public Guid ContainingListId { get; set; } = containingListId;
}
