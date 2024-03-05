import { Injectable, signal, WritableSignal } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IList } from "./list.model"

@Injectable({
  providedIn: 'root'
})
export class AnalyticsListsService {
  lists: WritableSignal<IList[]> = signal([])

  constructor(private readonly http: HttpClient) {
    this.fetchAll()
  }

  fetchAll() {
    return this.http.get<IList[]>("/api/lists?includePlayers=true")
      .subscribe(value => this.lists.set(value))
  }

  createByPlayers(name: string, playersIds: string[]) {
    return this.http.post<IList>("/api/players-lists", { name: name, ids: playersIds })
      .subscribe(newList => this.lists.update(
        lists => {
          lists.push(newList)
          return lists
        }))
  }

  fetchPlayerNames(playerNameStart: string) {
    return this.http.get<string[]>(`https://lichess.org/api/player/autocomplete?term=${playerNameStart}`)
  }
}
