import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

interface ProjectTile {
  id: number;
  title: string;
  subtitle: string;
  icon: string;
  route?: string;
}

@Component({
  selector: 'app-projects-cards',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NgbDropdownModule],
  templateUrl: './projects-cards.component.html',
  styleUrls: ['./projects-cards.component.scss']
})
export class ProjectsCardsComponent {
  projects: ProjectTile[] = [
    {
      id: 1,
      title: 'Archiv Digitalisierung',
      subtitle: 'Historische Dokumente digital erfassen und verschlagworten.',
      icon: 'assets/images/icons/Projects_128x128.gif',
      route: '/projects'
    },
    {
      id: 2,
      title: 'Ortschronik',
      subtitle: 'Meilensteine und Ereignisse der Region strukturiert erfassen.',
      icon: 'assets/images/icons/History_128x128.gif',
      route: '/project-cards'
    },
    {
      id: 3,
      title: 'Bildsammlung',
      subtitle: 'Fotos, Scans und Metadaten zentral zusammenfuehren.',
      icon: 'assets/images/icons/Mediathek_128x128.gif',
      route: '/projects'
    },
    {
      id: 4,
      title: 'Familienlinien',
      subtitle: 'Verwandtschaftsverlaeufe projektbezogen aufarbeiten.',
      icon: 'assets/images/icons/Familie_128x128.gif',
      route: '/projects'
    },
    {
      id: 5,
      title: 'Exkursion Planung',
      subtitle: 'Themenfahrten und Vortragsreihen koordinieren.',
      icon: 'assets/images/icons/Exkursion_128x128.gif',
      route: '/projects'
    },
    {
      id: 6,
      title: 'Foerderprojekte',
      subtitle: 'Antraege, Meilensteine und Ergebnisse dokumentieren.',
      icon: 'assets/images/icons/Logo_Stiftung_HUF_weiss.gif',
      route: '/projects'
    }
  ];
}
