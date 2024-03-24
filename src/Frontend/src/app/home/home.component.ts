import { Component } from '@angular/core';
import { AsyncPipe, NgOptimizedImage } from "@angular/common";
import { HealthCheckService } from "../health-check/health-check.service";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    AsyncPipe,
    NgOptimizedImage
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  healthStatus = this.healthCheckService.status$;

  constructor(private healthCheckService: HealthCheckService) {
  }

  checkStatus = () => {
    this.healthCheckService.checkHealth();
  }

  yearNow: number = new Date().getFullYear();
}
