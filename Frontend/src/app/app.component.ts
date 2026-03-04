import { Component } from '@angular/core';

@Component({
  selector: 'app-root',              // Name des HTML-Tags, über den die Komponente eingebunden wird
  standalone: false,
  templateUrl: './app.component.html', // Verweis auf die HTML-Datei
  styleUrls: ['./app.component.css']   // Verweis auf die CSS-Datei
})
export class AppComponent {
  title = 'my-app'; // Beispiel-Property, die im Template genutzt werden kann
}
