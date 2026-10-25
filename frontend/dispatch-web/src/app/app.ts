import { Component, signal } from '@angular/core';
import { IncidentList } from './components/incident-list/incident-list';
import { API_URL } from './config';
import { IncidentForm } from './components/incident-form/incident-form';

@Component({
  selector: 'app-root',
  imports: [IncidentList, IncidentForm],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('dispatch-web');
  protected readonly apiUrl = API_URL;
}
