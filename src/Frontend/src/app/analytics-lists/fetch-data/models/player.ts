import { Category } from "./category";
import { ITeam } from "./team";
import { GameResult } from "./game-result";
import { ITournament } from "./tournament";

export interface IPlayer {
  "nickname": string,
  "id": string,
  "ratingsHistories": IRatingHistory[],
  "gamesHistory": IGameHistory[],
  "statistics": IStatistic[],
  "tournaments": ITournamentStatistic[],
  "teams": ITeam[]
}

interface IRatingHistory {
  "category": Category,
  "ratingsPerDate": IRating[]
}

interface IRating {
  "rating": number,
  "actualityDate": string
}

interface IGameHistory {
  "nicknameForWhite": string,
  "nicknameForBlack": string,
  "gameResult": GameResult,
  "winnerRateChange": number,
  "loserChangeRate": number,
  "gameType": Category,
  "gameDate": string
}

interface IStatistic {
  "category": Category,
  "wins": number,
  "losses": number,
  "draws": number,
  "gamesAmount": number,
  "actualityDate": string
}

interface ITournamentStatistic {
  "tournament": ITournament,
  "score": number,
  "rank": number,
}
