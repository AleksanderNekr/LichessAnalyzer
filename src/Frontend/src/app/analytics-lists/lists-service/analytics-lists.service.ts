import { Injectable, signal, WritableSignal } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IList } from "./list.model"

@Injectable({
  providedIn: 'root'
})
export class AnalyticsListsService {
  lists: WritableSignal<IList[]> = signal([])

  constructor(private readonly http: HttpClient) {
    this.fetchAllLists()
  }

  fetchAllLists() {
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

  delete(listId: string) {
    return this.http.delete(`api/lists/${ listId }`)
      .subscribe(_ => this.lists.update(
        lists => lists.filter(l => l.id != listId)))
  }
}
