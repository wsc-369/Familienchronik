import { Component } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';

interface PortalTile {
  title: string;
  subtitle: string;
  icon: string;
  route?: string;
}

@Component({
  selector: 'app-portal-cards',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './portal-cards.component.html',
  styleUrls: ['./portal-cards.component.scss']
})
export class PortalCardsComponent {

  tiles: PortalTile[] = [
    {
      title: 'Ahnenforschung',
      subtitle: 'Stammbäume, Personen, Beziehungen',
      icon: 'bi-people',
      route: '/ahnen'
    },
    {
      title: 'About',
      subtitle: 'PDFs, Bilder, OCR',
      icon: 'bi-file-earmark-text',
      route: '/about'
    },
    {
      title: 'Uploads',
      subtitle: 'Bilder & Medien verwalten',
      icon: 'bi-images',
      route: '/fileUpload'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/familyTree'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Galerie',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Einstellungen',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/settings'
    },
    {
      title: 'Mitglieder',
      subtitle: 'System & Benutzer',
      icon: 'bi-gear',
      route: '/appUsers'
    }
    
  ];
}
