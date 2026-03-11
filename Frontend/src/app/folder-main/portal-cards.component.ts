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
      title: 'Kontakt',
      subtitle: 'Hast du Fragen oder Anregungen? Kontaktiere uns!',
      icon: 'assets/images/icons/Contact_128x128.gif',
      route: '/ahnen'
    },
    {
      id: 2,
      title: 'Stiftung- und Vereinsgeschichte',
      subtitle: 'Entstehung und Entwicklung der Stiftung und des Vereins',
      icon: 'assets/images/icons/History_128x128.gif',
      route: '/about'
    },
    {
      id: 3,
      title: 'Uploads',
      subtitle: 'Mitglidschaft, haben wir dein Interesse geweckt? Hier kannst du dich beim Verein anmelden.',
      icon: 'bi-images',
      route: '/fileUpload'
    },
    {
      id: 4,
      title: 'Familienchronik',
      subtitle: 'Familienchronik und Verwandtschaftsverbindungen',
      icon: 'assets/images/icons/Familie_128x128.gif',
      route: '/familyTree'
    },
    {
      id: 5,
      title: 'Projekte',
      subtitle: 'Versschiedene Themenbereiche, an denen wir arbeiten',
      icon: 'assets/images/icons/Projects_128x128.gif',
      route: '/project-cards'
    },
    {
      id: 6,
      title: 'Mediathek',
      subtitle: 'Dokumente, Schriften, Bilder',
      icon: 'assets/images/icons/Mediathek_128x128.gif',
      route: '/settings'
    },
    {
      id: 7,
      title: 'Intranet',
      subtitle: 'Informationen für unsere Vereinsmitglieder',
      icon: 'assets/images/icons/Intranet_128x128.gif',
      route: '/settings'
    },
    {
      id: 8,
      title: 'Exkursionen u. Veranstaltungen',
      subtitle: 'Uasflüge, Treffen, Vorträge und andere Veranstaltungen',
      icon: 'assets/images/icons/Exkursion_128x128.gif',
      route: '/settings'
    },
    {
      id: 9,
      title: 'Projelktförderung',
      subtitle: 'Stifung Heimat- und Familiengeschichte, Projektförderung',
      icon: 'assets/images/icons/Logo_Stiftung_HUF_weiss.gif',
      route: '/settings'
    },
    {
      id: 10,
      title: 'xxEinstellungen',
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
