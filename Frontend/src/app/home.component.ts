import { Component, HostListener } from '@angular/core';
import { RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CarouselComponent } from './controls/carousel/carousel.component';
import { HeaderComponent } from "./header.component";
import { CardComponent } from './controls/card/card.component';


@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [CommonModule, CarouselComponent, HeaderComponent, CardComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  isMenuOpen = false;
  scrolled = false;


  constructor(router: Router) {
    router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
      //  this.currentUrl = event.urlAfterRedirects;
        this.isMenuOpen = false;
      }
    });
  }

  @HostListener('window:scroll', [])
  onScroll(): void {
    this.scrolled = window.scrollY > 60;
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu(): void {
    this.isMenuOpen = false;
  }
}