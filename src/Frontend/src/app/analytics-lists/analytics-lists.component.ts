import { Component, computed, Output, signal } from '@angular/core';
import { AnalyticsListsService } from "./lists-service/analytics-lists.service";
import { DashboardAreaComponent } from "./dashboard-area/dashboard-area.component";
import { IList } from "./lists-service/list.model";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { CreateListModalComponent } from "./create-list-modal/create-list-modal.component";
import { ListsStorageService } from "./dashboard-area/storage-service/lists-storage.service";

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
              private readonly listsStorageService: ListsStorageService,
              protected readonly listsService: AnalyticsListsService) {
    let listJson = listsStorageService.getLastSelectedList()
    if (listJson !== null) {
      this.selectedList.set(JSON.parse(listJson))
    }
  }

  selectList(list: IList | null) {
    if (this.selectedList()?.id === list?.id) {
      return
    }
    this.prevSelectedList.set(this.selectedList())
    this.selectedList.set(list)
    this.listsStorageService.saveLastSelectedList(list)
  }

  markIfSelected(id: string) {
    return this.selectedList()?.id === id ? "bg-primary bg-opacity-10" : "";
  }

  createList() {
    this.modalService.open(CreateListModalComponent, { size: "xl" })
  }
}
