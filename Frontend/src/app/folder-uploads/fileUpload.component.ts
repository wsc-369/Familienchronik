// file-upload.component.ts
import { Component } from '@angular/core';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-file-upload',
  standalone: true,
  imports: [CommonModule, NgbModule],
  templateUrl: './fileUploadComponent.html',
  styleUrls: ['./fileUploadComponent.css']
})
export class FileUploadComponent {
  uploadProgress = 0;
  isUploading = false;
  isDragOver = false;

  constructor(private http: HttpClient) {}

  onDragOver(event: DragEvent) {
    event.preventDefault(); // wichtig, sonst wird das Drop-Event nicht ausgelöst
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;
  }

  onFileDropped(event: DragEvent) {
    event.preventDefault();
    this.isDragOver = false;

    if (event.dataTransfer && event.dataTransfer.files.length > 0) {
      this.uploadFile(event.dataTransfer.files[0]);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.uploadFile(input.files[0]);
    }
  }

  private uploadFile(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    this.isUploading = true;
    this.uploadProgress = 0;

    this.http.post('/api/upload', formData, {
      reportProgress: true,
      observe: 'events'
    }).subscribe({
      next: (event) => {
        if (event.type === HttpEventType.UploadProgress && event.total) {
          this.uploadProgress = Math.round(100 * event.loaded / event.total);
        } else if (event.type === HttpEventType.Response) {
          this.isUploading = false;
          this.uploadProgress = 100;
        }
      },
      error: () => {
        this.isUploading = false;
        this.uploadProgress = 0;
      }
    });
  }
}