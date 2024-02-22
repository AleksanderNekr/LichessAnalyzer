using Backend.DataManagement.LichessApi.LichessResponsesModels;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;

namespace Backend.DataManagement.LichessApi;

public class GetDataService(HttpClient httpClient, ILogger<GetDataService> logger)
{
    public async Task<IEnumerable<PlayerResponse>?> GetChessPlayersAsync(IEnumerable<string> ids,
                                                                         List<PlayerStat>    stats,
                                                                         List<PlayCategory>  categories,
                                                                         CancellationToken   cancellationToken = default)
    {
        using HttpResponseMessage resp = await httpClient.PostAsync("users",
                                                                    new StringContent(string.Join(',', ids)),
                                                                    cancellationToken);

        resp.EnsureSuccessStatusCode();
        var usersResponse = await resp.Content.ReadFromJsonAsync<List<UsersByIdResponse>>(cancellationToken);
        if (usersResponse is null)
        {
            return Enumerable.Empty<PlayerResponse>();
        }

        logger.LogDebug("Starting building players");
        List<PlayerResponse> composedPlayers = new(usersResponse.Count);
        composedPlayers = await BuildPlayersAsync(composedPlayers);

        logger.LogDebug("Finished building players");

        return composedPlayers;

        async Task<List<PlayerResponse>> BuildPlayersAsync(List<PlayerResponse> players)
        {
            foreach (UsersByIdResponse userResponse in usersResponse)
            {
                var ratingsHistories     = (IReadOnlyList<RatingHistory>)Enumerable.Empty<RatingHistory>();
                var gamesLists           = (IReadOnlyList<GamesList>)Enumerable.Empty<GamesList>();
                var statistics           = (IReadOnlyList<Statistic>)Enumerable.Empty<Statistic>();
                var tournamentStatistics = (IReadOnlyList<TournamentStatistic>)Enumerable.Empty<TournamentStatistic>();
                var teams                = (IReadOnlyList<TeamResponse>)Enumerable.Empty<TeamResponse>();
                foreach (PlayerStat stat in stats)
                {
                    switch (stat)
                    {
                        case PlayerStat.Ratings:
                            logger.LogDebug("Starting getting ratings histories");
                            ratingsHistories = await GetRatingsHistoriesAsync(userResponse.Id, categories, cancellationToken);

                            break;
                        case PlayerStat.GamesHistory:
                            logger.LogDebug("Starting getting games history");
                            gamesLists = GetGamesListsAsync();

                            break;
                        case PlayerStat.AllGameStats:
                            logger.LogDebug("Starting getting all games stats");
                            statistics = GetStatistics();

                            break;
                        case PlayerStat.TournamentsStats:
                            logger.LogDebug("Starting getting tournaments stats");
                            tournamentStatistics = GetTournamentStatistics();

                            break;
                        case PlayerStat.Teams:
                            logger.LogDebug("Starting getting teams");
                            teams = GetPlayerTeams();

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(stats));
                    }
                }

                PlayerResponse composedPlayer = new(
                    userResponse.Username,
                    userResponse.Id,
                    ratingsHistories,
                    gamesLists,
                    statistics,
                    tournamentStatistics,
                    teams);

                logger.LogDebug("Composed player: {@Player}", composedPlayer);
                players.Add(composedPlayer);
            }

            logger.LogDebug("Finished building players");

            return players;
        }
    }

    private IReadOnlyList<TeamResponse> GetPlayerTeams()
    {
        return (IReadOnlyList<TeamResponse>)Enumerable.Empty<TeamResponse>();
    }

    private IReadOnlyList<TournamentStatistic> GetTournamentStatistics()
    {
        return (IReadOnlyList<TournamentStatistic>)Enumerable.Empty<TournamentStatistic>();
    }

    private IReadOnlyList<Statistic> GetStatistics()
    {
        return (IReadOnlyList<Statistic>)Enumerable.Empty<Statistic>();
    }

    private IReadOnlyList<GamesList> GetGamesListsAsync()
    {
        return (IReadOnlyList<GamesList>)Enumerable.Empty<GamesList>();
    }

    private async Task<IReadOnlyList<RatingHistory>> GetRatingsHistoriesAsync(
        string                    userId,
        IEnumerable<PlayCategory> categories,
        CancellationToken         cancellationToken)
    {
        var ratings = await httpClient.GetFromJsonAsync<List<RatingHistoryResponse>>($"user/{userId}/rating-history",
                                                                                     cancellationToken);
        if (ratings is null)
        {
            return (IReadOnlyList<RatingHistory>)Enumerable.Empty<RatingHistory>();
        }

        List<RatingHistory> ratingHistories =
        [
            ..ratings.Where(r => Enum.TryParse(r.Category, out PlayCategory parsed)
                              && categories.Contains(parsed))
                     .Select(r => new RatingHistory(Enum.Parse<PlayCategory>(r.Category),
                                                    r.PointsPerDate.Select(BuildRating)))
        ];

        logger.LogDebug("Got ratings histories: {@Ratings}", ratingHistories);

        return ratingHistories;

        static RatingPerDate BuildRating(IReadOnlyList<int> pointPerDate)
        {
            return new RatingPerDate(pointPerDate[3],
                                     new DateOnly(year: pointPerDate[0],
                                                  month: pointPerDate[1] + 1,
                                                  day: pointPerDate[2]));
        }
    }

    public IReadOnlyList<TeamResponse> GetTeams(
        IEnumerable<string> ids,
        bool                withParticipants,
        bool                withTournaments)
    {
        return (IReadOnlyList<TeamResponse>)Enumerable.Empty<TeamResponse>();
    }
}
