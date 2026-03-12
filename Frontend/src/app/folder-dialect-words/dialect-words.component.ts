import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, finalize, map, switchMap, takeUntil } from 'rxjs/operators';
import { DialectWord } from '../api/api-interfaces';
import { DialectWordService } from '../folder-services/dialect-word.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-dialect-words',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dialect-words.component.html',
  styleUrls: ['./dialect-words.component.css']
})
export class DialectWordsComponent implements OnInit, OnDestroy {
  words: DialectWord[] = [];
  searchTerm = '';
  isLoading = false;
  errorMessage = '';
  private readonly backendBaseUrl = environment.apiUrl.replace(/\/api\/?$/, '');
  private readonly searchTerm$ = new Subject<string>();
  private readonly destroy$ = new Subject<void>();

  constructor(private readonly dialectWordService: DialectWordService) {}

  ngOnInit(): void {
    this.setupSearch();
    this.onSearchTermChange('');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.searchTerm$.complete();
  }

  trackByWordId(_: number, word: DialectWord): string {
    return word.id;
  }

  onSearchTermChange(term: string): void {
    this.searchTerm$.next(term);
  }

  resolveVoiceUrl(voice: string | null | undefined): string {
    if (!voice) {
      return '';
    }

    const trimmedVoice = voice.trim();
    if (!trimmedVoice) {
      return '';
    }

    if (/^https?:\/\//i.test(trimmedVoice)) {
      return trimmedVoice;
    }

    const normalizedPath = trimmedVoice.replace(/\\/g, '/').replace(/^\/+/, '');
    if (normalizedPath.toLowerCase().startsWith('resources/audio/dialect/')) {
      return `${this.backendBaseUrl}/${normalizedPath}`;
    }

    return `${this.backendBaseUrl}/resources/audio/dialect/${normalizedPath}`;
  }

  private setupSearch(): void {
    this.searchTerm$
      .pipe(
        debounceTime(300),
        map((term) => term.trim()),
        distinctUntilChanged(),
        switchMap((term) => {
          this.isLoading = true;
          this.errorMessage = '';

          const request$ = term
            ? this.dialectWordService.getDialectFilteredWords(term)
            : this.dialectWordService.getDialectWords();

          return request$.pipe(
            map((result) =>
              (Array.isArray(result) ? result : [result]).filter(
                (item): item is DialectWord => item !== null && item !== undefined
              )
            ),
            catchError(() => {
              this.errorMessage = 'Dialektwörter konnten nicht geladen werden.';
              return of([] as DialectWord[]);
            }),
            finalize(() => {
              this.isLoading = false;
            })
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe((words) => {
        this.words = words ?? [];
      });
  }
}
