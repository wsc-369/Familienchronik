import { Routes } from '@angular/router';
import { HomeComponent } from './home.component';
import { AppComponent } from './app.component';
import { AboutComponent } from './about.component';
import { FileUploadComponent } from './folder-uploads/fileUpload.component';
import { FamilyTreeComponent } from './folder-familys/famliy-tree.component';
import { ApplicationUserComponent } from './folder-application-users/applicationUser.component';
/* import { AboutComponent } from './about/about.component'; */

export const routes: Routes = [
  { path: '', component: HomeComponent },   // Startseite
  { path: 'about', component: AboutComponent },
  { path: 'fileUpload', component: FileUploadComponent },
  { path: 'familyTree', component: FamilyTreeComponent },
  { path: 'appUsers', component: ApplicationUserComponent },
  { path: '**', redirectTo: '' }            // Fallback: alles andere auf Home
];
