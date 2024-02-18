namespace Backend.DataManagement.Users.Entities;

public class User(Guid id, string name)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public int MaxPlayersInList { get; set; } = 100;

    public int MaxListsCount { get; set; } = 30;

    public List<AnalyticsList> AnalyticsLists { get; set; } = default!;

    public List<Team> OrganisedTeams { get; set; } = default!;
}
