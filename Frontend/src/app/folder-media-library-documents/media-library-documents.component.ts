import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, finalize, map, switchMap, takeUntil, tap } from 'rxjs/operators';
import { MediaLibraryDocument, SearchResult } from '../api/api-interfaces';
import { MediaLibraryDocumentService } from '../folder-services/media-library-document.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-media-library-documents',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './media-library-documents.component.html',
  styleUrls: ['./media-library-documents.component.css']
})
export class MediaLibraryDocumentsComponent implements OnInit, OnDestroy {
  documents: MediaLibraryDocument[] = [];
  searchTerm = '';
  suggestions: string[] = [];
  isLoading = false;
  errorMessage = '';
  totalCount = 0;
  pageIndex = 0;
  readonly pageSize = 10;

  private readonly backendBaseUrl = environment.apiUrl.replace(/\/api\/?$/, '');
  private readonly searchTerm$ = new Subject<string>();
  private readonly destroy$ = new Subject<void>();

  constructor(private readonly mediaLibraryDocumentService: MediaLibraryDocumentService) { }

  ngOnInit(): void {
    this.setupSearch();
    this.onSearchTermChange('');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.searchTerm$.complete();
  }

  trackByDocumentId(_: number, document: MediaLibraryDocument): string {
    return document.id;
  }

  onSearchTermChange(term: string): void {
    this.searchTerm$.next(term);
  }

  onSuggestionClick(suggestion: string): void {
    this.searchTerm = suggestion;
    this.pageIndex = 0;
    this.onSearchTermChange(suggestion);
    this.suggestions = [];
  }

  onNextPage(): void {
    if (!this.canGoNext) {
      return;
    }

    this.pageIndex += 1;
    this.onSearchTermChange(this.searchTerm);
  }

  onPreviousPage(): void {
    if (!this.canGoPrevious) {
      return;
    }

    this.pageIndex -= 1;
    this.onSearchTermChange(this.searchTerm);
  }

  get canGoPrevious(): boolean {
    return this.pageIndex > 0;
  }

  get canGoNext(): boolean {
    return (this.pageIndex + 1) * this.pageSize < this.totalCount;
  }

  resolveDocumentUrl(path: string | null | undefined): string {
    if (!path) {
      return '';
    }

    const trimmedPath = path.trim();
    if (!trimmedPath) {
      return '';
    }

    if (/^https?:\/\//i.test(trimmedPath)) {
      return trimmedPath;
    }

    const normalizedPath = trimmedPath.replace(/\\/g, '/').replace(/^\/+/, '');
    return `${this.backendBaseUrl}/${normalizedPath}`;
  }

  private setupSearch(): void {
    this.searchTerm$
      .pipe(
        debounceTime(300),
        map((term) => term.trim()),
        distinctUntilChanged(),
        tap((term) => {
          if (term.length >= 2) {
            this.mediaLibraryDocumentService
              .getSuggestions(term)
              .pipe(
                catchError(() => of([] as string[])),
                takeUntil(this.destroy$)
              )
              .subscribe((list) => {
                this.suggestions = list;
              });
          } else {
            this.suggestions = [];
          }
        }),
        switchMap((term) => {
          this.isLoading = true;
          this.errorMessage = '';

          return this.mediaLibraryDocumentService.searchDocuments(term, this.pageIndex, this.pageSize).pipe(
            map((result) => result ?? ({ results: [], totalCount: 0, pageIndex: this.pageIndex, pageSize: this.pageSize } as SearchResult)),
            catchError(() => {
              this.errorMessage = 'Dokumente konnten nicht geladen werden.';
              return of({ results: [], totalCount: 0, pageIndex: this.pageIndex, pageSize: this.pageSize } as SearchResult);
            }),
            finalize(() => {
              this.isLoading = false;
            })
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe((searchResult) => {
        this.documents = searchResult.results ?? [];
        this.totalCount = searchResult.totalCount ?? 0;
        this.pageIndex = searchResult.pageIndex ?? this.pageIndex;
      });
  }
}
