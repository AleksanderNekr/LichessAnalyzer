using Backend.DataManagement.LichessApi.ServiceResponsesModels;

namespace Backend.DataManagement.Analytics;

public interface IAnalyticsCacheService
{
    public Task<bool> CachePlayerAsync(PlayerResponse player);

    public Task<PlayerResponse?> ExtractPlayerAsync(string playerId);

    public Task<bool> CacheTeamAsync(TeamResponse team);

    public Task<TeamResponse?> ExtractTeamAsync(string playerId);
}
