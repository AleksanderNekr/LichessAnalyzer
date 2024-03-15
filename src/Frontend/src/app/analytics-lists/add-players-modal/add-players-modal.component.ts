import { Component, WritableSignal } from '@angular/core';
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { AnalyticsListsService } from "../lists-service/analytics-lists.service";
import { AuthService } from "../../auth/auth.service";
import { FetchDataService } from "../fetch-data/fetch-data.service";
import { IList } from "../lists-service/list.model";

@Component({
  selector: 'app-create-list-modal',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './add-players-modal.component.html',
})
export class AddPlayersModalComponent {
  searchResults: string[] = []
  playersIdsSearchResult: string[] = []
  lastSearchWasByPlayers: boolean | undefined
  playersIdsSelected: string[] = []
  private draftPlayersIdsSelected: string[] = []
  private list!: IList
  private callback!: () => void;

  constructor(protected readonly activeModal: NgbActiveModal,
              private readonly authService: AuthService,
              private readonly fetchService: FetchDataService,
              private readonly listsService: AnalyticsListsService) {
  }

  selectedSearchByName = true
  selectedSearchAmongPlayers = true
  searchFormGroup = new FormGroup({
    searchField: new FormControl(
      '', [ Validators.required, Validators.minLength(3), Validators.maxLength(25) ]),
  })

  setList(list: IList) {
    this.list = list
  }

  setSubmitCallback(callback: () => void) {
    this.callback = callback
  }

  handleSearch() {
    this.searchResults = []
    this.playersIdsSearchResult = []
    this.draftPlayersIdsSelected = []
    this.searchFormGroup.controls.searchField.setValue(this.searchFormGroup.controls.searchField.value?.trim() ?? '')
    if (this.searchFormGroup.invalid) {
      return
    }

    this.lastSearchWasByPlayers = false
    if (this.selectedSearchAmongPlayers) {
      this.lastSearchWasByPlayers = true
      if (this.selectedSearchByName) {
        this.fetchService.completePlayerNames(this.searchFormGroup.controls.searchField.value!)
          .subscribe(value => {
            console.log('Fetched: ', value)
            return this.playersIdsSearchResult = value;
          })
      }
    }
  }

  onSelectedPlayerChange(playerId: string) {
    if (this.draftPlayersIdsSelected.indexOf(playerId) >= 0) {
      this.draftPlayersIdsSelected = this.draftPlayersIdsSelected.filter(id => id !== playerId)
    } else {
      this.draftPlayersIdsSelected.push(playerId)
    }
  }

  handleSubmit() {
    if (this.authService.isAuthenticated()) {
      this.listsService.addPlayers(this.list, this.playersIdsSelected)
        .add(() => this.callback())
    } else {
      alert("Currently only authorized users allowed!")
    }

    this.activeModal.close('Players added to list')
  }

  saveDraftPlayersIds() {
    for (const id of this.draftPlayersIdsSelected) {
      if (this.playersIdsSelected.indexOf(id) >= 0) {
        continue
      }
      this.playersIdsSelected.push(id)
    }
  }

  unselectPlayerId(playerId: string) {
    this.playersIdsSelected = this.playersIdsSelected.filter(id => id !== playerId)
  }
}
