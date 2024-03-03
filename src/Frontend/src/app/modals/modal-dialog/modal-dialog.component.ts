import { Component } from '@angular/core';
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ButtonStyle } from "./button.style";

@Component({
  selector: 'app-modal-dialog',
  standalone: true,
  imports: [],
  templateUrl: './modal-dialog.component.html',
  styleUrl: './modal-dialog.component.css'
})
export class ModalDialogComponent {
  protected submitCallback: () => void = () => console.error("Submit callback is not overriden")

  protected modalTitle: string = "";
  protected modalBodyText: string = "";
  protected modalSubmitButtonText: string = "";
  protected modalCancelButtonText: string = "";

  protected modalOkBtnStyle: string = "btn-danger";

  constructor(protected readonly activeModal: NgbActiveModal) {
  }

  public setSubmitCallback(callback: () => void) {
    this.submitCallback = callback
  }

  public handleSubmit() {
    this.submitCallback()
    this.activeModal.close('OK clicked')
  }

  public setTexts(title: string, body: string, submitBtnText: string, cancelBtnText: string) {
    this.modalTitle = title
    this.modalBodyText = body
    this.modalSubmitButtonText = submitBtnText
    this.modalCancelButtonText = cancelBtnText
  }

  public setSubmitBtnStyle(style: ButtonStyle) {
    switch (style) {
      case ButtonStyle.Primary:
        this.modalOkBtnStyle = "btn-primary"
        break;
      case ButtonStyle.Danger:
        this.modalOkBtnStyle = "btn-danger"
        break;
    }
  }
}
