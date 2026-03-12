import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, finalize, map, switchMap, takeUntil } from 'rxjs/operators';
import { MediaLibraryDocument } from '../api/api-interfaces';
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
  isLoading = false;
  errorMessage = '';

  private readonly backendBaseUrl = environment.apiUrl.replace(/\/api\/?$/, '');
  private readonly searchTerm$ = new Subject<string>();
  private readonly destroy$ = new Subject<void>();

  constructor(private readonly mediaLibraryDocumentService: MediaLibraryDocumentService) {}

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
        switchMap((term) => {
          this.isLoading = true;
          this.errorMessage = '';

          const request$ = term
            ? this.mediaLibraryDocumentService.getMediaLibraryFilteredDocuments(term)
            : this.mediaLibraryDocumentService.getMediaLibraryDocuments();

          return request$.pipe(
            map((result) => result ?? []),
            catchError(() => {
              this.errorMessage = 'Dokumente konnten nicht geladen werden.';
              return of([] as MediaLibraryDocument[]);
            }),
            finalize(() => {
              this.isLoading = false;
            })
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe((documents) => {
        this.documents = documents;
      });
  }
}
