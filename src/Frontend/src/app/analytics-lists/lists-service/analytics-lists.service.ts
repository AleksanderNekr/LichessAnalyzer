import { effect, Injectable, signal, WritableSignal } from '@angular/core';
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

  addPlayers(list: IList, playersIdsSelected: string[]) {
    return this.http.post(`api/lists/${ list.id }/players`, playersIdsSelected)
      .subscribe(_ => this.lists.update(
        lists => {
          let players = list.listedPlayers ?? []
          for (const id of playersIdsSelected) {
            players.push({ id: id, containingListId: list.id })
          }
          list.listedPlayers = players

          let i = lists.findIndex(value => value.id === list.id)
          lists[i] = list

          return lists
        }))
  }

  delete(listId: string) {
    return this.http.delete(`api/lists/${ listId }`)
      .subscribe(_ => this.lists.update(
        lists => lists.filter(l => l.id != listId)))
  }

  updateName(listId: string, newName: string) {
    return this.http.put(`api/lists/${ listId }/${ newName }`, null)
      .subscribe(_ => this.lists.update(
        lists => {
          let changingList = lists.find(l => l.id === listId)
          changingList!.name = newName
          return lists
        }
      ))
  }
}
