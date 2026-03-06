import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

interface CardData {
  id: number;
  title: string;
  summary: string;
  imageUrl?: string;
  link?: string;
  tags?: string[];
}


@Component({
  selector: 'app-card',
  templateUrl: "./card.component.html",
  styleUrls: ['./card.component.scss'],
  imports: [CommonModule, RouterModule ]
})
export class CardComponent {
  @Input() data!: CardData;
  currentUrl = '';
  cards: CardData[] = [
    {
      id: 1,
      title: 'Veranstaltungshinweis',
      summary: 'Am 12. Mai: Open Day mit Demo & Q&A.',
      imageUrl: 'assets/images/service1.jpg',
      link: '/services',
      tags: ['news', 'digital']
    },
    {
      id: 2,
      title: 'Kontakt / Mitglied',
      summary: 'Kantaktaddresse, Mitgliedschaft',
      imageUrl: 'assets/images/event.jpg',
      tags: ['event', 'community']
    },
    {
      id: 3,
      title: 'Was amchen wir',
      summary: 'So nutzt du unsere Plattform effektiv.',
      link: '/guide',
      tags: ['howto', 'beginner']
    }
,
    {
      id: 5,
      title: 'Anleitung: Loslegen',
      summary: 'So nutzt du unsere Plattform effektiv.',
      link: '/guide',
      tags: ['howto', 'beginner']
    }
    
  ];
}