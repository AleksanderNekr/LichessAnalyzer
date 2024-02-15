using Backend.DataManagement.LichessApi.LichessResponsesModels;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;

namespace Backend.DataManagement.LichessApi;

public class GetDataService(HttpClient httpClient, ILogger<GetDataService> logger)
{
    public async Task<IEnumerable<PlayerResponse>?> GetChessPlayersAsync(IEnumerable<string> ids,
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

        List<PlayerResponse> composedPlayers = new(usersResponse.Count);
        foreach (UsersByIdResponse userResponse in usersResponse)
        {
            IReadOnlyList<RatingHistory>       ratingsHistories     = await GetRatingsHistoriesAsync(userResponse.Id, cancellationToken);
            IReadOnlyList<GamesList>           gamesLists           = GetGamesListsAsync();
            IReadOnlyList<Statistic>           statistics           = GetStatistics();
            IReadOnlyList<TournamentStatistic> tournamentStatistics = GetTournamentStatistics();
            IReadOnlyList<TeamResponse>        teams                = GetTeams();

            PlayerResponse composedPlayer = new(
                userResponse.Username,
                userResponse.Id,
                ratingsHistories,
                gamesLists,
                statistics,
                tournamentStatistics,
                teams);

            composedPlayers.Add(composedPlayer);
        }

        return composedPlayers;
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

    private async Task<IReadOnlyList<RatingHistory>> GetRatingsHistoriesAsync(string userId, CancellationToken cancellationToken)
    {
        var ratings = await httpClient.GetFromJsonAsync<List<RatingHistoryResponse>>($"user/{userId}/rating-history",
                                                                                     cancellationToken);
        if (ratings is null)
        {
            return (IReadOnlyList<RatingHistory>)Enumerable.Empty<RatingHistory>();
        }

        List<RatingHistory> ratingHistories = [ ];
        ratingHistories.AddRange(ratings.Select(ratingResponse => new RatingHistory(
                                                    ratingResponse.Category.ToCategoryEnum(),
                                                    ratingResponse.PointsPerDate.Select(BuildRating))));

        return ratingHistories;

        RatingPerDate BuildRating(IReadOnlyList<int> pointPerDate)
        {
            return new RatingPerDate(pointPerDate[3],
                                     new DateOnly(year: pointPerDate[0],
                                                  month: pointPerDate[1] + 1,
                                                  day: pointPerDate[2]));
        }
    }

    public IReadOnlyList<TeamResponse> GetTeams(params string[] ids)
    {
        return (IReadOnlyList<TeamResponse>)Enumerable.Empty<TeamResponse>();
    }
}
