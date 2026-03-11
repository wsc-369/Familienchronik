import { Component } from '@angular/core';
import { NgbDropdownModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { MobileMenuComponent } from "./mobile/mobile-menu.component";
import { AuthService } from './folder-services/auth.service';

@Component({
  selector: 'app-root',              // Name des HTML-Tags, über den die Komponente eingebunden wird
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    NgbDropdownModule
  ], templateUrl: './app.component.html'
})
export class AppComponent {
  currentYear = new Date().getFullYear();

  constructor(
    private readonly offcanvas: NgbOffcanvas,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  openMobileMenu() {
    this.offcanvas.open(MobileMenuComponent, { position: 'start' });
  }

  openOffcanvas() {
    this.offcanvas.open(AppComponent, { position: 'start' });
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
