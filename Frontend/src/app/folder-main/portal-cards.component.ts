import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

interface PortalTile {
  id: number;
  title: string;
  subtitle: string;
  icon: string;
  route?: string;
}

@Component({
  selector: 'app-portal-cards',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NgbDropdownModule],
  templateUrl: './portal-cards.component.html',
  styleUrls: ['./portal-cards.component.scss']
})
export class PortalCardsComponent {

  tiles: PortalTile[] = [
    {
      id: 1,
      title: 'Ahnenforschung',
      subtitle: 'Stammbäume, Personen, Beziehungen',
      icon: 'bi-people',
      route: '/ahnen'
    },
    {
      id: 2,
      title: 'About',
      subtitle: 'PDFs, Bilder, OCR',
      icon: 'bi-file-earmark-text',
      route: '/about'
    },
    {
      id: 3,
      title: 'Uploads',
      subtitle: 'Bilder & Medien verwalten',
      icon: 'bi-images',
      route: '/fileUpload'
    },
    {
      id: 4,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/familyTree'
    },
    {
      id: 5,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 6,
      title: 'Galerie',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 7,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 8,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 9,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 10,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 11,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 12,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 13,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 14,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 15,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 16,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 17,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 18,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 19,
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      id: 20,
      title: 'Mitglieder',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/appUsers'
    }

  ];
}
