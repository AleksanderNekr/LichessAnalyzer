import { Component, Input, signal, WritableSignal } from '@angular/core';
import { IList } from "../lists-service/list.model";
import { AnalyticsListsService } from "../lists-service/analytics-lists.service";
import { DashContentComponent } from "./dash-content/dash-content.component";
import { AddPlayersService } from "./dash-content/add-players.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { PrintContentComponent } from "./print-content/print-content.component";

@Component({
  selector: 'app-dashboard-area',
  standalone: true,
  imports: [ DashContentComponent ],
  templateUrl: './dashboard-area.component.html',
  styleUrl: './dashboard-area.component.css'
})
export class DashboardAreaComponent {
  @Input({ required: true }) selectedList: WritableSignal<IList | null> = signal(null)
  @Input({ required: true }) prevSelectedList: WritableSignal<IList | null> = signal(null)

  constructor(private readonly listsService: AnalyticsListsService,
              private readonly addPlayersService: AddPlayersService,
              private readonly modalService: NgbModal,) {
  }

  exportPlayersDataHandle() {
    if (this.selectedList() === null) {
      return
    }
    this.listsService.exportPlayersData(this.selectedList()!)
  }

  addPlayersHandle() {
    this.addPlayersService.addPlayersHandle(this.selectedList(), () => {
      this.selectedList()
    })
  }

  printDashboardHandle() {
    let modal = this.modalService.open(PrintContentComponent, { fullscreen: true })
    modal.componentInstance.setSelectedList(this.selectedList)
  }
}
