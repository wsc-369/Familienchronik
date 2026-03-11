import { Routes } from '@angular/router';
import { HomeComponent } from './home.component';
import { AboutComponent } from './about.component';
import { FileUploadComponent } from './folder-uploads/fileUpload.component';
import { FamilyTreeComponent } from './folder-familys/famliy-tree.component';
import { ApplicationUserComponent } from './folder-application-users/applicationUser.component';
import { PortalCardsComponent } from './folder-main/portal-cards.component';
import { ProjectsCardsComponent } from './folder-main/projects-cards.component';
import { LoginComponent } from './folder-auth/login.component';
import { LogoutComponent } from './folder-auth/logout.component';
import { ForgotPasswordComponent } from './folder-auth/forgot-password.component';
import { ChangePasswordComponent } from './folder-auth/change-password.component';
import { authGuard } from './folder-services/auth.guard';
/* import { AboutComponent } from './about/about.component'; */

export const routes: Routes = [
  { path: '', component: HomeComponent },   // Startseite
  { path: 'login', component: LoginComponent },
  { path: 'logout', component: LogoutComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'change-password', component: ChangePasswordComponent, canActivate: [authGuard] },
  { path: 'portal-cards', component: PortalCardsComponent },
  { path: 'projects', component: ProjectsCardsComponent },
  { path: 'project-cards', component: ProjectsCardsComponent },
  { path: 'about', component: AboutComponent },
  { path: 'fileUpload', component: FileUploadComponent, canActivate: [authGuard] },
  { path: 'familyTree', component: FamilyTreeComponent, canActivate: [authGuard] },
  { path: 'appUsers', component: ApplicationUserComponent, canActivate: [authGuard] },

  { path: '**', redirectTo: '' }            // Fallback: alles andere auf Home
];
