import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { IRatingForecastModel } from "./rating-forecast-model";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class RatingsForecastService {

  constructor(private readonly http: HttpClient) {
  }

  predictNextRatings(knownRatings: IRatingForecastModel[], horizon: number): Observable<IRatingForecastModel[]> {
    return this.http.post<IRatingForecastModel[]>(
      "/api/ratings-forecast",
      {
        ratings: knownRatings,
        horizon: horizon,
      });
  }
}
