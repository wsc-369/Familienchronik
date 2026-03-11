import { CommonModule } from "@angular/common";
import { Component, signal } from "@angular/core";
import { NavigationEnd, Router, RouterLink } from "@angular/router";
import { AuthService } from './folder-services/auth.service';

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
    readonly isAuthenticated$;

    constructor(private readonly router: Router, private readonly authService: AuthService) {
        this.isAuthenticated$ = this.authService.isAuthenticated$;
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

    logout(): void {
        this.authService.logout().subscribe(() => {
            this.closeMenu();
            this.router.navigate(['/login']);
        });
    }
}
