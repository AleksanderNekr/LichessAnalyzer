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

  create() {
    return this.http.post<IList>("/api/players-lists",
      {
        name: "TestList-2",
        ids: [ "AleksanderNekr", "Yuran11" ]
      }).subscribe(value => this.lists.update(
      l => {
        l.push(value)
        return l
      }))
  }
}
