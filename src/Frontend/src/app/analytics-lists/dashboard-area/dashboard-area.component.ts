import {
  Component,
  effect,
  Input,
  signal,
  WritableSignal
} from '@angular/core';
import { IList } from "../lists-service/list.model";
import { KtdGridLayout, KtdGridModule } from "@katoid/angular-grid-layout";
import { LineGraphComponent } from "./graphs/line-graph/line-graph.component";

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
  public dates: string[] = [ '2023-01-01', '2024-01-01', '2024-01-21' ]
  public rates: number[] = [ 1500, 1623, 1711 ]

  updateLayoutHandle(layout: KtdGridLayout) {
    this.layout = layout;
  }

  constructor() {
    effect(() => {
      localStorage.setItem(this.prevSelectedList()?.id + '-layout', JSON.stringify(this.layout))
      this.ngOnInit()
    });
  }

  ngOnInit() {
    let layoutJson = localStorage.getItem(this.selectedList()?.id + '-layout')
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
