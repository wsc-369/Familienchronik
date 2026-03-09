import { Component, ViewChild, TemplateRef, AfterViewInit } from '@angular/core';
import { ToastService } from './toast.service';
@Component({
  selector: 'app-global-toast-templates',
  standalone: true,
  template: `
    <ng-template #successToast>
      <div class="toast-body">
        <strong>OK!</strong> Aktion wurde erfolgreich ausgeführt.      
      </div>
    </ng-template>

    <ng-template #errorToast>
      <div class="toast-body">
        <strong>Fehler!</strong> Aktion ist fehlgeschlagen.
      </div>
    </ng-template>

    <ng-template #cancelToast>
      <div class="toast-body">
        <strong>Abgebrochen!</strong> Aktion wurde abgebrochen.
      </div>
    </ng-template>

    <ng-template #successUpdateToast>
      <div class="toast-body">
        <strong>Erfolg!</strong> Daten wurden aktualisiert.
      </div>
    
    </ng-template>
        <ng-template #errorUpdateToast>
      <div class="toast-body">
        <strong>Fehler!</strong> Aktualisierung fehlgeschlagen.
      </div>
    </ng-template>
  `
})
export class GlobalToastTemplatesComponent implements AfterViewInit {
  @ViewChild('successToast') successToast!: TemplateRef<any>;
  @ViewChild('errorToast') errorToast!: TemplateRef<any>;
  @ViewChild('cancelToast') cancelToast!: TemplateRef<any>;
  @ViewChild('successUpdateToast') successUpdateToast!: TemplateRef<any>;
  @ViewChild('errorUpdateToast') errorUpdateToast!: TemplateRef<any>;

  constructor(private toastService: ToastService) { }

  ngAfterViewInit() {
    this.toastService.registerTemplates({
      success: this.successToast,
      error: this.errorToast,
      cancel: this.cancelToast,
      successSave: this.successUpdateToast,
      errorSave: this.errorUpdateToast
    });
  }
}
