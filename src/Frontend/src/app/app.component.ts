import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HealthCheckService } from "./health-check/health-check.service";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ CommonModule, RouterOutlet ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Frontend';
  healthStatus = this.healthCheckService.status$;

  constructor(private healthCheckService: HealthCheckService) {
  }

  checkStatus = () => {
    this.healthCheckService.checkHealth();
  }

}
