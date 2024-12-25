import { Component, effect, Input, Output, signal, WritableSignal } from "@angular/core";
import { KtdGridLayout, KtdGridModule } from "@katoid/angular-grid-layout";
import { LineGraphComponent } from "../graphs/line-graph/line-graph.component";
import { PieChartComponent } from "../graphs/pie-chart/pie-chart.component";
import { ListsStorageService } from "../storage-service/lists-storage.service";
import { IList, IListedPlayer } from "../../lists-service/list.model";
import { AnalyticsListsService } from "../../lists-service/analytics-lists.service";
import { Subscription } from "rxjs";
import { Stat } from "../../fetch-data/models/stat";
import { Category } from "../../fetch-data/models/category";
import { FetchDataService } from "../../fetch-data/fetch-data.service";
import { IPlayer } from "../../fetch-data/models/player";
import { AddPlayersService } from "./add-players.service";
import { ForecastRatingsModalComponent } from "../forecast-ratings-modal/forecast-ratings-modal.component";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: "app-dash-content",
  standalone: true,
  imports: [
    KtdGridModule,
    LineGraphComponent,
    PieChartComponent,
  ],
  templateUrl: "./dash-content.component.html",
})
export class DashContentComponent {
  @Input({ required: true }) selectedList: WritableSignal<IList | null> = signal(null);
  @Input() prevSelectedList: WritableSignal<IList | null> = signal(null);
  @Input() forPrint: boolean = false;

  cols = 25;
  rowHeight = 50;
  gap = 10;

  public ratingsDates = [ new Date("2023-01-01"), new Date("2024-01-01"), new Date("2024-01-21") ];
  public playersStats: { data: { name: Date, value: number }[]; name: string; type: "line" }[] = [];
  private updateSub: Subscription | null = null;

  public gamesDates = [ new Date("2023-01-01"), new Date("2024-01-01"), new Date("2024-01-21") ];
  public gamesStats: { data: { name: Date, value: number }[]; name: string; type: "line" }[] = [];
  public tournamentsActivityData: { name: string; value: number }[] = [];

  layout: KtdGridLayout = [];

  constructor(private readonly fetchDataService: FetchDataService,
              private readonly listsStorageService: ListsStorageService,
              private readonly listsService: AnalyticsListsService,
              private readonly addPlayersService: AddPlayersService,
              private readonly modalService: NgbModal) {
    effect(() => {
      listsStorageService.saveDashboardLayout(this.prevSelectedList()?.id + "-layout", this.layout);
      this.ngOnInit();
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.updateCurrentLayout();
    if (this.selectedList()?.listedPlayers) {
      this.updateSub = this.updateData();
    }
  }

  ngOnDestroy() {
    this.updateSub?.unsubscribe();
  }

  private updateData() {
    let ids: string[] = [];
    for (let player of this.selectedList()?.listedPlayers!) {
      ids.push(player.id);
    }

    this.playersStats = [];
    this.gamesStats = [];
    this.tournamentsActivityData = [];
    return this.fetchDataService.fetchPlayers(ids, [ Stat.Ratings, Stat.TournamentsStats ], [ Category.Classical ])
      .subscribe(players => {
        let rateDates: Date[] = [];
        let gameDates: Date[] = [];

        for (const player of players) {
          this.updatePlayersStats(player, rateDates);
          this.updateGamesStats(player, gameDates);
          this.updateTournamentsActivity(player);
        }
        this.ratingsDates = rateDates.sort((date1, date2) => date1.getTime() - date2.getTime());
        this.gamesDates = gameDates.sort((date1, date2) => date1.getTime() - date2.getTime());
      });
  }

  private updatePlayersStats(player: IPlayer, rateDates: Date[]) {
    let rates: { name: Date, value: number }[] = [];
    if (!player.ratingsHistories[0]) {
      return;
    }
    for (let rating of player.ratingsHistories[0].ratingsPerDate) {
      let rateData = { name: new Date(rating.actualityDate), value: rating.rating };
      rates.push(rateData);
      rateDates.push(rateData.name);
    }
    this.playersStats.push({
      name: player.nickname,
      data: rates,
      type: "line",
    });
  }

  private updateGamesStats(player: IPlayer, gameDates: Date[]) {
    let games: { name: Date, value: number }[] = [];
    if (!player.statistics[0]) {
      return;
    }
    for (let stat of player.statistics) {
      let gameData = { name: new Date(stat.actualityDate), value: stat.gamesAmount };
      games.push(gameData);
      gameDates.push(gameData.name);
    }
    this.gamesStats.push({
      name: player.nickname,
      data: games,
      type: "line",
    });
  }

  updateCurrentLayout() {
    let layoutJson = this.listsStorageService.getDashboardLayout(this.selectedList()?.id + "-layout");
    if (layoutJson === null || layoutJson === "[]") {
      this.layout = [
        { id: "0", x: 17, y: 0, w: 8, h: 4 },
        { id: "1", x: 0, y: 4, w: 11, h: 3 },
        { id: "2", x: 11, y: 4, w: 14, h: 3 },
        { id: "3", x: 0, y: 0, w: 17, h: 4 },
      ];

      return;
    }

    this.layout = JSON.parse(layoutJson);
  }

  private updateTournamentsActivity(player: IPlayer) {
    if (!player.tournaments[0] || this.foundPlayer(player)) {
      return;
    }
    this.tournamentsActivityData.push({ name: player.nickname, value: player.tournaments.length });
  }

  foundPlayer(player: IPlayer) {
    return this.tournamentsActivityData.findIndex(value => value.name == player.nickname) >= 0;
  }

  updateLayoutHandle(layout: KtdGridLayout) {
    this.layout = layout;
  }

  removePlayerHandle(player: IListedPlayer) {
    if (this.selectedList() === null) {
      return;
    }
    this.listsService.removePlayer(this.selectedList()!, player)
      .add(() => {
        this.selectedList();
        this.updateData();
      });
  }

  addPlayersHandle() {
    this.addPlayersService.addPlayersHandle(this.selectedList(), () => {
      this.selectedList();
      this.updateData();
    });
  }

  forecastRatingsHandle() {
    let modal = this.modalService.open(ForecastRatingsModalComponent);
    modal.componentInstance.setCurrentPLayersRatings(this.ratingsDates, this.playersStats);
  }
}
