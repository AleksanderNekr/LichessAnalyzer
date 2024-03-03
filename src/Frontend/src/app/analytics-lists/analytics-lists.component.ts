import { Component, computed, Output, signal } from '@angular/core';
import { AuthService } from "../auth/auth.service";
import { AnalyticsListsService } from "./lists-service/analytics-lists.service";
import { DashboardAreaComponent } from "./dashboard-area/dashboard-area.component";
import { IList } from "./lists-service/list.model";

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
  lists = computed(() => this.listsService.lists());

  private readonly _selected: IList | null = null
  @Output() selectedList = signal(this._selected)

  constructor(private readonly authService: AuthService,
              protected readonly listsService: AnalyticsListsService) {
    let listJson = localStorage.getItem("lastSelected")
    if (listJson !== null) {
      this.selectedList.set(JSON.parse(listJson))
    }
  }


  selectList(list: IList) {
    this.selectedList.set(list)
    localStorage.setItem("lastSelected", JSON.stringify(list))
  }

  markIfSelected(id: string) {
    return this.selectedList()?.id === id ? "bg-primary bg-opacity-10" : "";
  }

  createList() {
    if (this.authService.isAuthenticated()) {
      this.listsService.create()
    } else {
      alert("Currently only authorized users allowed!")
    }
  }
}
