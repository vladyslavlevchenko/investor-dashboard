import { Component } from '@angular/core';
import { SecuritiesComponent } from '../securities/securities.component';

@Component({
  selector: 'app-dashboard',
  imports: [SecuritiesComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {}
