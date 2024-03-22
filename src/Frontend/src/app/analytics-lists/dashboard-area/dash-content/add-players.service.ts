import { Injectable } from '@angular/core';
import { AddPlayersModalComponent } from "../../add-players-modal/add-players-modal.component";
import { IList } from "../../lists-service/list.model";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Injectable({
  providedIn: 'root'
})
export class AddPlayersService {
  constructor(private readonly modalService: NgbModal,) {
  }

  addPlayersHandle(list: IList | null, submitCallback: () => void) {
    if (!list) {
      return
    }
    let modal = this.modalService.open(AddPlayersModalComponent, { size: "xl" })
    modal.componentInstance.setList(list)
    modal.componentInstance.setSubmitCallback(submitCallback)
  }
}
