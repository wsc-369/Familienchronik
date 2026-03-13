import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { Person, PersonPortrait } from '../api/api-interfaces';
import { PersonPortraitService } from '../folder-services/person-portrait.service';

@Component({
  selector: 'app-person-portraits',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './person-portraits.component.html',
  styleUrls: ['./person-portraits.component.css']
})
export class PersonPortraitsComponent implements OnInit {
  portraits: PersonPortrait[] = [];
  selectedPortrait: PersonPortrait = this.createEmptyPortrait();
  isLoading = false;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  constructor(private readonly personPortraitService: PersonPortraitService) {}

  ngOnInit(): void {
    this.loadPortraits();
  }

  trackByPortraitId(index: number, item: PersonPortrait): string {
    return item.id || `portrait-${index}`;
  }

  onCreateNew(): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.selectedPortrait = this.createEmptyPortrait();
  }

  onSelectPortrait(portrait: PersonPortrait): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.selectedPortrait = this.clonePortrait(portrait);
  }

  onSavePortrait(): void {
    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload = this.normalizePortrait(this.selectedPortrait);
    const request$ = payload.id
      ? this.personPortraitService.updatePersonPortrait(payload.id, payload)
      : this.personPortraitService.createPersonPortrait(payload);

    request$
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: (savedPortrait) => {
          this.successMessage = 'PersonPortrait wurde gespeichert.';
          this.selectedPortrait = this.clonePortrait(savedPortrait);
          this.loadPortraits(savedPortrait.id);
        },
        error: () => {
          this.errorMessage = 'PersonPortrait konnte nicht gespeichert werden.';
        }
      });
  }

  onDeletePortrait(): void {
    if (!this.selectedPortrait.id) {
      this.onCreateNew();
      return;
    }

    const confirmed = window.confirm('Soll dieses PersonPortrait wirklich geloescht werden?');
    if (!confirmed) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.personPortraitService
      .deletePersonPortrait(this.selectedPortrait.id)
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: () => {
          this.successMessage = 'PersonPortrait wurde geloescht.';
          this.selectedPortrait = this.createEmptyPortrait();
          this.loadPortraits();
        },
        error: () => {
          this.errorMessage = 'PersonPortrait konnte nicht geloescht werden.';
        }
      });
  }

  private loadPortraits(selectId?: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.personPortraitService
      .getPersonPortraits()
      .pipe(finalize(() => {
        this.isLoading = false;
      }))
      .subscribe({
        next: (portraits) => {
          this.portraits = portraits ?? [];

          if (selectId) {
            const selected = this.portraits.find((item) => item.id === selectId);
            if (selected) {
              this.selectedPortrait = this.clonePortrait(selected);
            }
          }
        },
        error: () => {
          this.errorMessage = 'PersonPortraits konnten nicht geladen werden.';
        }
      });
  }

  private createEmptyPortrait(): PersonPortrait {
    return {
      id: '',
      person: this.createEmptyPerson(),
      title: '',
      pdfName: '',
      remarks: '',
      createDate: new Date(),
      updateDate: new Date(),
      active: true
    };
  }

  private createEmptyPerson(): Person {
    return {
      Id: '',
      personId: '',
      firstName: '',
      familyName: '',
      genderStatus: 0,
      genderStatusName: '',
      active: true
    };
  }

  private clonePortrait(portrait: PersonPortrait): PersonPortrait {
    return {
      ...portrait,
      createDate: portrait.createDate ? new Date(portrait.createDate) : new Date(),
      updateDate: portrait.updateDate ? new Date(portrait.updateDate) : new Date(),
      person: {
        ...this.createEmptyPerson(),
        ...(portrait.person ?? this.createEmptyPerson())
      }
    };
  }

  private normalizePortrait(portrait: PersonPortrait): PersonPortrait {
    const person = {
      ...this.createEmptyPerson(),
      ...(portrait.person ?? this.createEmptyPerson())
    };

    return {
      ...portrait,
      id: portrait.id?.trim(),
      title: portrait.title?.trim(),
      pdfName: portrait.pdfName?.trim(),
      remarks: portrait.remarks?.trim(),
      createDate: portrait.createDate ? new Date(portrait.createDate) : new Date(),
      updateDate: new Date(),
      person: {
        ...person,
        Id: person.Id?.trim(),
        personId: person.personId?.trim(),
        firstName: person.firstName?.trim(),
        familyName: person.familyName?.trim(),
        genderStatusName: person.genderStatusName?.trim()
      }
    };
  }
}
