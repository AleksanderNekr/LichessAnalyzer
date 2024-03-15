import { Injectable } from '@angular/core';
import { KtdGridLayout } from "@katoid/angular-grid-layout";
import { IList } from "../../lists-service/list.model";

@Injectable({
  providedIn: 'root'
})
export class ListsStorageService {

  getDashboardLayout(dashboardId: string) {
    return localStorage.getItem(dashboardId)
  }

  saveDashboardLayout(dashboardId: string, layout: KtdGridLayout) {
    localStorage.setItem(dashboardId, JSON.stringify(layout))
  }

  deleteDashboardLayout(dashboardId: string) {
    localStorage.removeItem(dashboardId)
  }

  saveLastSelectedList(list: IList | null) {
    if (list === null) {
      localStorage.removeItem("lastSelected")
      return
    }
    localStorage.setItem("lastSelected", JSON.stringify(list))
  }

  getLastSelectedList() {
    return localStorage.getItem("lastSelected")
  }
}
