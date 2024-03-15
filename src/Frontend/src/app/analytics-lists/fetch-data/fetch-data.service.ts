import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";

import { IPlayer } from "./models/player"
import { Stat } from "./models/stat";
import { Category } from "./models/category";

@Injectable({
  providedIn: 'root'
})
export class FetchDataService {

  constructor(private readonly http: HttpClient) {
  }

  completePlayerNames(playerNameStart: string) {
    return this.http.get<string[]>(`https://lichess.org/api/player/autocomplete?term=${ playerNameStart }`)
  }

  fetchPlayers(ids: string[], stats: Stat[], categories: Category[]) {
    const queryParams = [];
    if (ids.length > 0) {
      queryParams.push(`ids=${ ids.join('&ids=') }`);
    }
    if (stats.length > 0) {
      stats.forEach(stat => {
        queryParams.push(`withStats=${ stat }`);
      });
    }
    if (categories.length > 0) {
      categories.forEach(category => {
        queryParams.push(`withCategories=${ category }`);
      });
    }
    const queryString = queryParams.length > 0 ? `?${ queryParams.join('&') }` : '';
    const url = `/api/players${ queryString }`;
    return this.http.get<IPlayer[]>(url);
  }
}
