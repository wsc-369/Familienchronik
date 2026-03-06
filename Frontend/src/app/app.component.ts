import { Component } from '@angular/core';
import { NgbPagination, NgbPaginationEllipsis, NgbPaginationFirst, NgbPaginationLast, NgbPaginationNext, NgbPaginationNumber, NgbPaginationPrevious, NgbPaginationPages, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { RouterOutlet } from "@angular/router";
import { MobileMenuComponent } from "./mobile/mobile-menu.component";

@Component({
  selector: 'app-root',              // Name des HTML-Tags, über den die Komponente eingebunden wird
  standalone: true,
  imports: [
    RouterOutlet
  ], templateUrl: './app.component.html'
})
export class AppComponent {
  currentYear = new Date().getFullYear();

  constructor(private offcanvas: NgbOffcanvas) { }

  openMobileMenu() {
    this.offcanvas.open(MobileMenuComponent, { position: 'start' });
  }

  openOffcanvas() {
    this.offcanvas.open(AppComponent, { position: 'start' });
  }

  logout() {
    console.log('User abgemeldet');
    // dein Logout-Logik, z. B. Router.navigate(['/login'])
  }
}
