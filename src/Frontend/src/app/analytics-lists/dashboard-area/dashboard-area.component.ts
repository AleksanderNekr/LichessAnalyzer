import { Component, Input, OnDestroy, OnInit, signal, WritableSignal } from '@angular/core';
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
export class DashboardAreaComponent implements OnInit, OnDestroy {
  @Input() selectedList: WritableSignal<IList | null> = signal(null)
  cols = 25
  rowHeight = 50
  gap = 10

  layout: KtdGridLayout = []

  updateLayoutHandle(layout: KtdGridLayout) {
    this.layout = layout;
  }

  ngOnInit() {
    console.log('on init ', this.selectedList()?.id)
    let layoutJson = localStorage.getItem(this.selectedList()?.id + '-layout')
    if (layoutJson === null) {
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

  ngOnDestroy() {
    console.log('on destroy ', this.selectedList()?.id)
    localStorage.setItem(this.selectedList()?.id + '-layout', JSON.stringify(this.layout))
  }
}
