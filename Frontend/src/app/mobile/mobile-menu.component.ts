import { Component } from '@angular/core';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';

@Component({
  template: `
    <div class="offcanvas-header">
      <h5 class="offcanvas-title">Menü</h5>
      <button type="button" class="btn-close" (click)="close()" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
      <nav class="nav flex-column">
        <a routerLink="/dashboard" class="nav-link" (click)="close()">Dashboard</a>
        <a routerLink="/users" class="nav-link" (click)="close()">Benutzer</a>
      </nav>
    </div>
  `
})
export class MobileMenuComponent {
  constructor(private activeOffcanvas: NgbActiveOffcanvas) {}

  close() {
    this.activeOffcanvas.close();
  }
}