import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NgbCarousel } from "@ng-bootstrap/ng-bootstrap";
import { NgbCarouselModule } from '@ng-bootstrap/ng-bootstrap';

interface Slide {
  id: number;
  title: string;
  text: string;
  img: string;
}

@Component({
 selector: 'app-carousel',
  standalone: true,
  templateUrl: './carousel.component.html',
  styleUrls: ['./carousel.component.scss'],
  imports: [CommonModule, NgbCarouselModule]
})
export class CarouselComponent {

 slides: Slide[] = [
    {
      id: 1,
      title: 'Willkommen bei uns',
      text: 'Wir gestalten digitale Lösungen — schnell, modern, verlässlich.',
      img: 'assets/images/16d0a5c8-5d8e-4b18-8d0b-86b8a88c41bb.jpg'
    },
    {
      id: 2,
      title: 'Neue Features',
      text: 'Jetzt mit Dark Mode und schneller Suche.',
      img: 'assets/images/2fad402b-9352-45af-960c-edd28e2ac151.jpg'
    },
    {
      id: 3,
      title: 'Community',
      text: 'Tausche dich mit anderen Nutzern aus — aktiv und offen.',
      img: 'assets/images/14432f64-f7c5-4836-845b-847c40ca5e77.jpg'
    }
  ];
}

