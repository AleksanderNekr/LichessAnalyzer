using System.Runtime.CompilerServices;
using System.Text.Json;
using Backend.DataManagement.LichessApi.LichessResponsesModels;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;

namespace Backend.DataManagement.LichessApi;

public class LichessDataService(HttpClient httpClient, ILogger<LichessDataService> logger)
{
    private const int TeamsLimit                  = 10;
    private const int PlayersLimit                = 500;
    private const int DefaultTeamTournamentsLimit = 100;

    public async Task<IEnumerable<PlayerResponse>> GetChessPlayersAsync(IEnumerable<string> ids,
                                                                        CancellationToken   cancellationToken = default)
    {
        ids = ids.Take(PlayersLimit);
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
                IReadOnlyList<RatingHistory>       ratingsHistories     = await GetRatingsHistoriesAsync(userResponse.Id, cancellationToken);
                IReadOnlyList<GamesList>           gamesLists           = GetGamesListsAsync();
                IReadOnlyList<Statistic>           statistics           = GetStatistics();
                IReadOnlyList<TournamentStatistic> tournamentStatistics = GetTournamentStatistics();
                IReadOnlyList<TeamResponse>        teams                = GetPlayerTeams();

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

    private async Task<IReadOnlyList<RatingHistory>> GetRatingsHistoriesAsync(
        string            userId,
        CancellationToken cancellationToken)
    {
        var ratings = await httpClient.GetFromJsonAsync<List<RatingHistoryResponse>>($"user/{userId}/rating-history",
                                                                                     cancellationToken);
        if (ratings is null)
        {
            return (IReadOnlyList<RatingHistory>)Enumerable.Empty<RatingHistory>();
        }

        List<RatingHistory> ratingHistories =
        [
            ..ratings.Where(r => Enum.TryParse<PlayCategory>(r.Category, out _))
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

    public async Task<IReadOnlyList<TeamResponse>> GetTeamsAsync(
        ICollection<string> ids,
        bool                withParticipants = true,
        bool                withTournaments = true,
        CancellationToken   cancellationToken = default)
    {
        ids = (ICollection<string>)ids.Take(TeamsLimit);
        HttpResponseMessage resp          = null!;
        List<TeamResponse>  composedTeams = new(ids.Count);

        foreach (string teamId in ids)
        {
            resp = await httpClient.GetAsync($"team/{teamId}",
                                             cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                continue;
            }

            var teamResponse = await resp.Content.ReadFromJsonAsync<TeamByIdResponse>(cancellationToken);
            if (teamResponse is null)
            {
                logger.LogWarning("Team with ID: {Id} not found", teamId);

                continue;
            }

            (List<string>? participants, List<TeamTournamentResponse>? tournaments) = await FetchCollectionsAsync(teamId);

            composedTeams.Add(new TeamResponse(
                                  teamResponse.Id,
                                  teamResponse.Name,
                                  teamResponse.Leader.Name,
                                  participants ?? [ ],
                                  tournaments  ?? [ ]));
        }

        resp.Dispose();

        return composedTeams;

        async Task<(List<string>? participants, List<TeamTournamentResponse>? tournaments)> FetchCollectionsAsync(string teamId)
        {
            List<string>?                 participants = null;
            List<TeamTournamentResponse>? tournaments  = null;

            if (withParticipants)
            {
                participants = new List<string>(PlayersLimit);
                await foreach (TeamParticipantResponse response in GetParticipantsAsync(teamId, cancellationToken))
                {
                    participants.Add(response.ParticipantName);
                }
            }

            if (withTournaments)
            {
                tournaments = new List<TeamTournamentResponse>(DefaultTeamTournamentsLimit);
                await foreach (TeamTournamentResponse response in GetTeamTournamentsAsync(teamId, cancellationToken))
                {
                    tournaments.Add(response);
                }
            }

            return (participants, tournaments);
        }
    }

    private async IAsyncEnumerable<TeamParticipantResponse> GetParticipantsAsync(
        string teamId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting getting JSON-stream...");
        Stream ndjsonStream = await httpClient.GetStreamAsync($"team/{teamId}/users",
                                                              cancellationToken);
        using StreamReader ndjsonReader = new(ndjsonStream);

        string? json               = await ndjsonReader.ReadLineAsync(cancellationToken);
        var     participantCounter = 1;
        while (json is not null && participantCounter <= PlayersLimit)
        {
            logger.LogDebug("Read json line: {Json}", json);

            yield return JsonSerializer.Deserialize<TeamParticipantResponse>(json)
                      ?? new TeamParticipantResponse(string.Empty);

            json = await ndjsonReader.ReadLineAsync(cancellationToken);
            participantCounter++;
        }
    }

    private async IAsyncEnumerable<TeamTournamentResponse> GetTeamTournamentsAsync(
        string teamId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting getting JSON-stream...");
        Stream ndjsonStream = await httpClient.GetStreamAsync($"team/{teamId}/swiss",
                                                              cancellationToken);
        using StreamReader ndjsonReader = new(ndjsonStream);

        string? json = await ndjsonReader.ReadLineAsync(cancellationToken);
        while (json is not null)
        {
            logger.LogDebug("Read json line: {Json}", json);

            yield return JsonSerializer.Deserialize<TeamTournamentResponse>(json)
                      ?? new TeamTournamentResponse(string.Empty, string.Empty, DateTime.MinValue);

            json = await ndjsonReader.ReadLineAsync(cancellationToken);
        }
    }
}
