import { Component } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { AnalyticsListsService } from "../lists-service/analytics-lists.service";

@Component({
  selector: 'app-edit-list-modal',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './edit-list-modal.component.html',
})
export class EditListModalComponent {
  private _oldListName: string | null = null
  nameField = new FormControl(this._oldListName,
    [ Validators.required, Validators.minLength(3), Validators.maxLength(255) ])
  private _listId: string | null = null

  constructor(protected readonly activeModal: NgbActiveModal,
              private readonly listsService: AnalyticsListsService) {
  }

  setOldName(oldName: string) {
    this._oldListName = oldName
    this.nameField.setValue(oldName)
  }

  setListId(listId: string) {
    this._listId = listId
  }

  handleSubmit() {
    if (this.nameField.value === null) {
      return
    }
    this.nameField.setValue(this.nameField.value?.trim())
    if (this.nameField.invalid) {
      return;
    }
    this.listsService.updateName(this._listId!, this.nameField.value)
    this.activeModal.close('List name updated')
  }

  nameInvalid() {
    return this.nameField.invalid || this.nameField.value === this._oldListName
  }
}
