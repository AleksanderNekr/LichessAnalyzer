import { Component, effect, Input, signal, WritableSignal } from '@angular/core';
import { IList } from "../lists-service/list.model";
import { KtdGridLayout, KtdGridModule } from "@katoid/angular-grid-layout";
import { LineGraphComponent } from "./graphs/line-graph/line-graph.component";
import { FetchDataService } from "../fetch-data/fetch-data.service";
import { Stat } from "../fetch-data/models/stat";
import { Category } from "../fetch-data/models/category";
import { Subscription } from "rxjs";
import { ListsStorageService } from "./storage-service/lists-storage.service";

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
  public dates = [ new Date('2023-01-01'), new Date('2024-01-01'), new Date('2024-01-21') ]
  public playersStats: { data: number[]; name: string; type: 'line' }[] = []
  private updateSub: Subscription | null = null

  updateLayoutHandle(layout: KtdGridLayout) {
    this.layout = layout;
  }

  constructor(private readonly fetchDataService: FetchDataService,
              private readonly listsStorageService: ListsStorageService) {
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
    return this.fetchDataService.fetchPlayers(ids, [ Stat.Ratings ], [ Category.Classical ])
      .subscribe(players => {
        let dates: Date[] = []
        for (const player of players) {
          let rates: number[] = []
          for (let rating of player.ratingsHistories[0].ratingsPerDate) {
            rates.push(rating.rating)
            dates.push(new Date(rating.actualityDate))
          }

          this.playersStats.push({
            name: player.nickname,
            data: rates,
            type: "line"
          })
        }
        this.dates = dates.sort((date1, date2) => date1.getTime() - date2.getTime())
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
}
