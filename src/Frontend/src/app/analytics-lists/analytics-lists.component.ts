import { Component, computed, Output, signal } from '@angular/core';
import { AnalyticsListsService } from "./lists-service/analytics-lists.service";
import { DashboardAreaComponent } from "./dashboard-area/dashboard-area.component";
import { IList } from "./lists-service/list.model";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { CreateListModalComponent } from "./create-list-modal/create-list-modal.component";

@Component({
  selector: 'app-analytics-lists',
  standalone: true,
  imports: [
    DashboardAreaComponent
  ],
  templateUrl: './analytics-lists.component.html',
  styleUrl: './analytics-lists.component.css'
})
export class AnalyticsListsComponent {
  private readonly _selected: IList | null = null
  @Output() selectedList = signal(this._selected)
  @Output() prevSelectedList = signal(this._selected)

  constructor(private readonly modalService: NgbModal,
              protected readonly listsService: AnalyticsListsService) {
    let listJson = localStorage.getItem("lastSelected")
    if (listJson !== null) {
      this.selectedList.set(JSON.parse(listJson))
    }
  }


  selectList(list: IList) {
    this.prevSelectedList.set(this.selectedList())
    this.selectedList.set(list)
    localStorage.setItem("lastSelected", JSON.stringify(list))
  }

  markIfSelected(id: string) {
    return this.selectedList()?.id === id ? "bg-primary bg-opacity-10" : "";
  }

  createList() {
    this.modalService.open(CreateListModalComponent, { size: "xl" })
  }
}
