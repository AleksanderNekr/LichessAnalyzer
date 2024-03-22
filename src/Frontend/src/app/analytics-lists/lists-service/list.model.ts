export interface IList {
  "id": string
  "name": string
  "creatorId": string
  "listedPlayers": IListedPlayer[]
}

export interface IListedPlayer {
  "id": string
  "containingListId": string
}
