import { Component } from '@angular/core';
import { AuthService } from "../auth/auth.service";
import { ModalDialogComponent } from "../modals/modal-dialog/modal-dialog.component";
import { ButtonStyle } from "../modals/modal-dialog/button.style";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css'
})
export class AccountComponent {
  constructor(protected readonly authService: AuthService,
              private readonly modalService: NgbModal) {
  }

  deleteAccount() {
    let modal = this.modalService.open(ModalDialogComponent)
    modal.componentInstance.setSubmitCallback(() => this.authService.deleteAccount())
    modal.componentInstance.setTexts(
      "Confirm Account Deletion",
      "Are you really want to Delete your account? Your Lichess account will NOT be deleted",
      "Yes, Delete",
      "No, keep as is"
    )
    modal.componentInstance.setSubmitBtnStyle(ButtonStyle.Danger)
  }
}
