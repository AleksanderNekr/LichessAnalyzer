import { Component, Input } from '@angular/core';
import { IList } from "../lists-service/list.model";

@Component({
  selector: 'app-dashboard-area',
  standalone: true,
  imports: [],
  templateUrl: './dashboard-area.component.html',
  styleUrl: './dashboard-area.component.css'
})
export class DashboardAreaComponent {
  @Input() selectedList: IList | null = null
}
