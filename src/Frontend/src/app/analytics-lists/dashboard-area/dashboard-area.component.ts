import { Component, Input } from '@angular/core';
import { IList } from "../lists-service/list.model";
import { KtdGridLayout, KtdGridModule } from "@katoid/angular-grid-layout";

@Component({
  selector: 'app-dashboard-area',
  standalone: true,
  imports: [
    KtdGridModule
  ],
  templateUrl: './dashboard-area.component.html',
  styleUrl: './dashboard-area.component.css'
})
export class DashboardAreaComponent {
  @Input() selectedList: IList | null = null
  cols = 3
  rowHeight = 50
  layout: KtdGridLayout = [
    { id: '0', x: 5, y: 0, w: 2, h: 3 },
    { id: '1', x: 2, y: 2, w: 1, h: 2 },
    { id: '2', x: 3, y: 7, w: 1, h: 2 },
    { id: '3', x: 2, y: 0, w: 3, h: 2 },
  ]

  stopEventPropagation($event: MouseEvent) {
    $event.preventDefault();
    $event.stopPropagation();
  }

  removeItem(id: string) {
    this.layout = this.ktdArrayRemoveItem(this.layout, (item) => item.id === id);
  }

  ktdArrayRemoveItem<T>(array: T[], condition: (item: T) => boolean) {
    const arrayCopy = [ ...array ];
    const index = array.findIndex((item) => condition(item));
    if (index > -1) {
      arrayCopy.splice(index, 1);
    }
    return arrayCopy;
  }
}
