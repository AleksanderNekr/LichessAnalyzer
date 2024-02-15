namespace Backend.DataManagement.Users.Entities;

public class AnalyticsList(Guid id, string name, Guid userId)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public Guid UserId { get; set; } = userId;

    public User Creator { get; set; } = default!;

    public IEnumerable<Player> ListedPlayers { get; set; } = default!;
}
