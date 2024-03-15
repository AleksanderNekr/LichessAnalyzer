import { Component, Input, signal, WritableSignal } from '@angular/core';
import { DashContentComponent } from "../dash-content/dash-content.component";
import { IList } from "../../lists-service/list.model";

@Component({
  selector: 'app-print-content',
  standalone: true,
  imports: [
    DashContentComponent,
  ],
  templateUrl: './print-content.component.html',
})
export class PrintContentComponent {
  selectedList: WritableSignal<IList | null> = signal(null)
  headerVisible: boolean = true

  setSelectedList(listSignal: WritableSignal<IList | null>) {
    this.selectedList.set(listSignal())
  }

  printHandle() {
    this.headerVisible = false
    window.print()
    this.headerVisible = true
  }
}
