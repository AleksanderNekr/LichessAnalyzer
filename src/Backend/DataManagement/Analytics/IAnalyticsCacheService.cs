using Backend.DataManagement.Users.Entities;

namespace Backend.DataManagement.Analytics;

public interface IAnalyticsCacheService
{
    public Task<bool> CachePlayerAsync(Player player);

    public Task<Player?> ExtractPlayerAsync(string playerId);

    public Task<bool> CacheTeamAsync(Team team);

    public Task<Team?> ExtractTeamAsync(string playerId);
}
