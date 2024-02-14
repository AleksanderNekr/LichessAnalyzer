using System.Text.Json.Serialization;

namespace Backend.DataManagement.LichessApi.ServiceResponsesModels;

public record PlayerResponse(
    [property: JsonPropertyName("nickname")]         string                             Nickname,
    [property: JsonPropertyName("id")]               string                             Id,
    [property: JsonPropertyName("ratingsHistories")] IReadOnlyList<RatingHistory>       RatingsHistories,
    [property: JsonPropertyName("gamesHistory")]     IReadOnlyList<GamesList>           GamesLists,
    [property: JsonPropertyName("statistics")]       IReadOnlyList<Statistic>           Statistics,
    [property: JsonPropertyName("tournaments")]      IReadOnlyList<TournamentStatistic> Tournaments,
    [property: JsonPropertyName("teams")]            IReadOnlyList<TeamResponse>        Teams
);

public record GamesList(
    [property: JsonPropertyName("nicknameForWhite")] string       NicknameForWhite,
    [property: JsonPropertyName("nicknameForBlack")] string       NicknameForBlack,
    [property: JsonPropertyName("gameResult")]       GameResult   GameResult,
    [property: JsonPropertyName("winnerRateChange")] int          WinnerRateChange,
    [property: JsonPropertyName("loserChangeRate")]  int          LoserChangeRate,
    [property: JsonPropertyName("gameType")]         PlayCategory GameType,
    [property: JsonPropertyName("gameDate")]         DateTime     GameDate
);

public record RatingHistory(
    [property: JsonPropertyName("category")]      PlayCategory Category,
    [property: JsonPropertyName("rating")]        int          Rating,
    [property: JsonPropertyName("actualityDate")] DateTime     ActualityDate
);

public record Statistic(
    [property: JsonPropertyName("category")]      PlayCategory Category,
    [property: JsonPropertyName("wins")]          int          Wins,
    [property: JsonPropertyName("losses")]        int          Losses,
    [property: JsonPropertyName("draws")]         int          Draws,
    [property: JsonPropertyName("gamesAmount")]   int          GamesAmount,
    [property: JsonPropertyName("actualityDate")] DateTime     ActualityDate
);

public record TournamentStatistic(
    [property: JsonPropertyName("tournament")]          Tournament Tournament,
    [property: JsonPropertyName("wins")]                int        Wins,
    [property: JsonPropertyName("losses")]              int        Losses,
    [property: JsonPropertyName("draws")]               int        Draws,
    [property: JsonPropertyName("meanOpponentsRating")] double     MeanOpponentsRating,
    [property: JsonPropertyName("rating")]              int        Rating
);

public record Tournament(
    [property: JsonPropertyName("id")]                string   Id,
    [property: JsonPropertyName("name")]              string   Name,
    [property: JsonPropertyName("organiserNickname")] string   OrganiserNickname,
    [property: JsonPropertyName("date")]              DateTime Date
);
