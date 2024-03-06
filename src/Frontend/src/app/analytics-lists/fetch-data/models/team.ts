import { ITournament } from "./tournament";

export interface ITeam {
  "id": string,
  "name": string,
  "organiserNickname": string,
  "participants": string[],
  "tournaments": ITournament[]
}
