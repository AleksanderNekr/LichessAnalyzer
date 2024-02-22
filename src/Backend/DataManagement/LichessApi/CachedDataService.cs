using Backend.DataManagement.Analytics;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;

namespace Backend.DataManagement.LichessApi;

public class CachedDataService(
    DataService                dataService,
    IAnalyticsCacheService     cacheService,
    ILogger<CachedDataService> logger)
{
    public async Task<IEnumerable<PlayerResponse>> GetChessPlayersAsync(
        List<string>       ids,
        List<PlayerStat>   stats,
        List<PlayCategory> categories,
        CancellationToken  cancellationToken = default)
    {
        List<PlayerResponse> fetchedPlayers =
            (await Task.WhenAll(ids.Select(cacheService.ExtractPlayerAsync)))
           .Where(response => response is not null)
           .ToList()!;

        List<string> fetchedPlayersIds = fetchedPlayers.Select(player => player.Id)
                                                       .ToList();
        logger.LogDebug("Got players IDs from cache ({Count}): {@Ids}",
                        fetchedPlayersIds.Count,
                        fetchedPlayersIds);

        if (AllFetched())
        {
            logger.LogDebug("All players fetched from cache");
            return fetchedPlayers;
        }

        IEnumerable<PlayerResponse> notFetchedPlayers = await CacheNotFetchedPlayers();

        return fetchedPlayers.UnionBy(notFetchedPlayers, player => player.Id, StringComparer.Ordinal);

        async Task<IEnumerable<PlayerResponse>> CacheNotFetchedPlayers()
        {
            logger.LogDebug("Starting caching not fetched players...");
            IEnumerable<string> notFetchedIds = ids.Where(id => !fetchedPlayersIds.Contains(id));
            IEnumerable<PlayerResponse> players = (await dataService.GetChessPlayersAsync(
                                                       notFetchedIds,
                                                       cancellationToken)).ToList();

            IEnumerable<Task<bool>> cacheTasks = players.Select(cacheService.CachePlayerAsync);

            bool[] results     = await Task.WhenAll(cacheTasks);
            int    failedCount = results.Count(succeeded => !succeeded);
            if (failedCount > 0)
            {
                logger.LogWarning("{Count} cache player tasks failed", failedCount);
            }

            logger.LogDebug("All cache tasks succeeded");

            return players;
        }

        bool AllFetched()
        {
            return fetchedPlayersIds.Count == ids.Count;
        }
    }

    public async Task<IEnumerable<TeamResponse>> GetTeamsAsync(
        List<string> ids,
        bool         withParticipants,
        bool         withTournaments)
    {
        List<TeamResponse> fetchedTeams =
            (await Task.WhenAll(ids.Select(cacheService.ExtractTeamAsync)))
           .Where(response => response is not null)
           .ToList()!;

        List<string> fetchedTeamsIds = fetchedTeams.Select(team => team.Id)
                                                   .ToList();

        if (AllFetched())
        {
            return fetchedTeams;
        }

        IEnumerable<TeamResponse> notFetchedTeams = await CacheNotFetched();

        return fetchedTeams.UnionBy(notFetchedTeams, team => team.Id, StringComparer.Ordinal);

        async Task<IEnumerable<TeamResponse>> CacheNotFetched()
        {
            IEnumerable<string> notFetchedIds = ids.Where(id => !fetchedTeamsIds.Contains(id));
            IEnumerable<TeamResponse> teams = dataService.GetTeams(notFetchedIds, withParticipants, withTournaments)
                                                         .ToList();

            IEnumerable<Task<bool>> cacheTasks = teams.Select(cacheService.CacheTeamAsync);

            bool[] results     = await Task.WhenAll(cacheTasks);
            int    failedCount = results.Count(succeeded => !succeeded);
            if (failedCount > 0)
            {
                logger.LogWarning("{Count} cache team tasks failed", failedCount);
            }

            logger.LogDebug("All cache tasks succeeded");

            return teams;
        }

        bool AllFetched()
        {
            return fetchedTeamsIds.Count == ids.Count;
        }
    }
}
