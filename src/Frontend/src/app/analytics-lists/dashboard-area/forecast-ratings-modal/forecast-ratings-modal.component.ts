import { Component } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { LineGraphComponent } from "../graphs/line-graph/line-graph.component";
import { RatingsForecastService } from "../../ratings-forecast/ratings-forecast.service";
import { IRatingForecastModel } from "../../ratings-forecast/rating-forecast-model";
import { formatDate } from "@angular/common";

@Component({
  selector: "app-forecast-ratings-modal",
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    LineGraphComponent,
  ],
  templateUrl: "./forecast-ratings-modal.component.html",
})
export class ForecastRatingsModalComponent {
  private datesUpdated: boolean = false;

  protected dates: Date[] = [];
  protected stats: { data: { name: Date, value: number }[]; name: string; type: "line" }[] = [];

  constructor(private readonly forecastService: RatingsForecastService) {
  }

  setCurrentPLayersRatings(ratingDates: Date[], playersStats: {
    data: { name: Date, value: number }[];
    name: string;
    type: "line"
  }[]) {
    this.dates = ratingDates;
    this.stats = playersStats;

    let newDates: Date[] | null = null;
    playersStats.forEach(stat => {
      let knownRatings: IRatingForecastModel[] = [];
      for (let i = 0; i < stat.data.length; i++) {
        knownRatings.push({
          rating: stat.data[i].value,
          date: formatDate(ratingDates[i], "yyyy-MM-dd", "en-US"),
        });
      }

      this.forecastService.predictNextRatings(knownRatings, 30)
        .subscribe(predictions => {
          newDates = predictions.map(value => new Date(value.date));

          let newRatings = predictions.map(value => {
            return { name: new Date(value.date), value: value.rating };
          });
          playersStats.push({ name: stat.name, type: stat.type, data: newRatings });
        })
        .add(() => {
          this.dates = this.dates.concat(newDates?.filter(date => !this.dates.includes(date)) ?? []);
          this.stats.push(...playersStats);
        });
    });
  }
}
