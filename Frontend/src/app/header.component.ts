import { CommonModule } from "@angular/common";
import { Component, signal } from "@angular/core";
import { NavigationEnd, Router, RouterLink } from "@angular/router";

@Component({
    selector: 'app-header',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.css']
})
export class HeaderComponent {
    isMenuOpen = false;
    scrolled = false;
    currentUrl = '';

    constructor(router: Router) {
        router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                this.currentUrl = event.urlAfterRedirects;
                this.isMenuOpen = false;
            }
        });
    }


    //protected readonly title = signal('app_ahnenforschung-angular-frontend');

    toggleMenu(): void {
        this.isMenuOpen = !this.isMenuOpen;
    }

    closeMenu(): void {
        this.isMenuOpen = false;
    }
}
