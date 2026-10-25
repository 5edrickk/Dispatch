import { Component, signal } from '@angular/core';
import { IncidentList } from './components/incident-list/incident-list';
import { API_URL } from './config';

@Component({
  selector: 'app-root',
  imports: [IncidentList],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('dispatch-web');
  protected readonly apiUrl = API_URL;
}
