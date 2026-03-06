import { Routes } from '@angular/router';
import { HomeComponent } from './home.component';
import { AppComponent } from './app.component';
import { AboutComponent } from './about.component';
import { FileUploadComponent } from './folder-uploads/fileUpload.component';
/* import { AboutComponent } from './about/about.component'; */

export const routes: Routes = [
  { path: '', component: HomeComponent },   // Startseite
  { path: 'about', component: AboutComponent },
  { path: 'fileUpload', component: FileUploadComponent },
  { path: '**', redirectTo: '' }            // Fallback: alles andere auf Home
];
