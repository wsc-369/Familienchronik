import { CommonModule } from "@angular/common";
import { Component, signal } from "@angular/core";
import { Router } from "@angular/router";

@Component({
    selector: 'app-applicationUser',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './applicationUser.component.html',
    styleUrls: ['./applicationUser.component.css']
})
export class ApplicationUserComponent {
    constructor(router: Router) {
    }
    protected readonly title = signal('app_ahnenforschung-angular-frontend');
}
