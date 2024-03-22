import { Component, Input, signal, WritableSignal } from '@angular/core';
import { DashContentComponent } from "../dash-content/dash-content.component";
import { IList } from "../../lists-service/list.model";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-print-content',
  standalone: true,
  imports: [
    DashContentComponent,
  ],
  templateUrl: './print-content.component.html',
})
export class PrintContentComponent {
  constructor(private readonly activeModal: NgbActiveModal) {
  }
  selectedList: WritableSignal<IList | null> = signal(null)

  setSelectedList(listSignal: WritableSignal<IList | null>) {
    this.selectedList.set(listSignal())
  }

  printHandle() {
    document.getElementById("header")?.remove()
    print()
    this.activeModal.close('print')
  }
}
