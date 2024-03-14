import { Component, effect, Input, signal, WritableSignal } from '@angular/core';
import { IList, IListedPlayer } from "../lists-service/list.model";
import { KtdGridLayout, KtdGridModule } from "@katoid/angular-grid-layout";
import { LineGraphComponent } from "./graphs/line-graph/line-graph.component";
import { FetchDataService } from "../fetch-data/fetch-data.service";
import { Stat } from "../fetch-data/models/stat";
import { Category } from "../fetch-data/models/category";
import { Subscription } from "rxjs";
import { ListsStorageService } from "./storage-service/lists-storage.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { AddPlayersModalComponent } from "../add-players-modal/add-players-modal.component";
import { AnalyticsListsService } from "../lists-service/analytics-lists.service";
import { IPlayer } from "../fetch-data/models/player";

@Component({
  selector: 'app-dashboard-area',
  standalone: true,
  imports: [
    KtdGridModule,
    LineGraphComponent
  ],
  templateUrl: './dashboard-area.component.html',
  styleUrl: './dashboard-area.component.css'
})
export class DashboardAreaComponent {
  @Input() selectedList: WritableSignal<IList | null> = signal(null)
  @Input() prevSelectedList: WritableSignal<IList | null> = signal(null)

  cols = 25
  rowHeight = 50
  gap = 10

  layout: KtdGridLayout = []
  public ratingsDates = [ new Date('2023-01-01'), new Date('2024-01-01'), new Date('2024-01-21') ]
  public playersStats: { data: number[]; name: string; type: 'line' }[] = []
  private updateSub: Subscription | null = null

  public gamesDates = [ new Date('2023-01-01'), new Date('2024-01-01'), new Date('2024-01-21') ]
  public gamesStats: { data: number[]; name: string; type: 'line' }[] = []

  updateLayoutHandle(layout: KtdGridLayout) {
    this.layout = layout;
  }

  constructor(private readonly fetchDataService: FetchDataService,
              private readonly listsStorageService: ListsStorageService,
              private readonly modalService: NgbModal,
              private readonly listsService: AnalyticsListsService,) {
    effect(() => {
      listsStorageService.saveDashboardLayout(this.prevSelectedList()?.id + '-layout', this.layout)
      this.ngOnInit()
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.updateCurrentLayout()
    if (this.selectedList()?.listedPlayers) {
      this.updateSub = this.updateData()
    }
  }

  ngOnDestroy() {
    this.updateSub?.unsubscribe()
  }

  private updateData() {
    let ids: string[] = []
    for (let player of this.selectedList()?.listedPlayers!) {
      ids.push(player.id)
    }

    this.playersStats = []
    this.gamesStats = []
    return this.fetchDataService.fetchPlayers(ids, [ Stat.Ratings ], [ Category.Classical ])
      .subscribe(players => {
        let rateDates: Date[] = []
        let gameDates: Date[] = []

        for (const player of players) {
          this.updatePlayersStats(player, rateDates)
          this.updateGamesStats(player, gameDates)
        }
        this.ratingsDates = rateDates.sort((date1, date2) => date1.getTime() - date2.getTime())
        this.gamesDates = gameDates.sort((date1, date2) => date1.getTime() - date2.getTime())
      })
  }

  private updatePlayersStats(player: IPlayer, rateDates: Date[]) {
    let rates: number[] = []
    if (!player.ratingsHistories[0]) {
      return
    }
    for (let rating of player.ratingsHistories[0].ratingsPerDate) {
      rates.push(rating.rating)
      rateDates.push(new Date(rating.actualityDate))
    }
    this.playersStats.push({
      name: player.nickname,
      data: rates,
      type: "line"
    })
  }

  private updateGamesStats(player: IPlayer, gameDates: Date[]) {
    let games: number[] = []
    if (!player.statistics[0]) {
      return
    }
    for (let stat of player.statistics) {
      games.push(stat.gamesAmount)
      gameDates.push(new Date(stat.actualityDate))
    }
    this.gamesStats.push({
      name: player.nickname,
      data: games,
      type: "line"
    })
  }

  updateCurrentLayout() {
    let layoutJson = this.listsStorageService.getDashboardLayout(this.selectedList()?.id + '-layout')
    if (layoutJson === null || layoutJson === '[]') {
      this.layout = [
        { id: '0', x: 17, y: 0, w: 8, h: 4 },
        { id: '1', x: 0, y: 4, w: 11, h: 3 },
        { id: '2', x: 11, y: 4, w: 14, h: 3 },
        { id: '3', x: 0, y: 0, w: 17, h: 4 },
      ]

      return
    }

    this.layout = JSON.parse(layoutJson)
  }

  addPlayersHandle() {
    if (!this.selectedList()) {
      return
    }
    let modal = this.modalService.open(AddPlayersModalComponent, { size: "xl" })
    modal.componentInstance.setList(this.selectedList()!)
    modal.componentInstance.setSubmitCallback(() => {
      this.selectedList()
      this.updateData()
    })
  }

  removePlayerHandle(player: IListedPlayer) {
    if (this.selectedList() === null) {
      return
    }
    this.listsService.removePlayer(this.selectedList()!, player)
      .add(() => {
        this.selectedList()
        this.updateData()
      })
  }
}
