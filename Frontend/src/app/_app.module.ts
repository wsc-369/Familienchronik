import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap/alert';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'; 
import { RouterModule } from '@angular/router';

import {
  NgbPagination,
  NgbPaginationEllipsis,
  NgbPaginationFirst,
  NgbPaginationLast,
  NgbPaginationNext,
  NgbPaginationNumber,
  NgbPaginationPrevious,
  NgbPaginationPages,
} from '@ng-bootstrap/ng-bootstrap/pagination';

// Hier importierst du die gewünschten ngx-bootstrap Module
;
import { AppComponent } from './app.component';
import { HomeComponent } from './home.component';

@NgModule({
  declarations: [
   // AppComponent // deine eigenen Komponenten
  ],
  imports: [
    BrowserModule,
    NgbModule, 
    
    NgbPagination,
    NgbPaginationEllipsis,
    NgbPaginationFirst,
    NgbPaginationLast,
    NgbPaginationNext,
    NgbPaginationNumber,
    NgbPaginationPrevious,
    NgbPaginationPages,
    NgbAlert,
    RouterModule.forRoot([ 
       { path: '', component: HomeComponent }, 
 /*      { path: 'about', component: AboutComponent }, 
      { path: 'contact', component: ContactComponent } */
  ])

  ],
  providers: [],
 // bootstrap: [AppComponent] // Startkomponente
})
export class AppModule { }
