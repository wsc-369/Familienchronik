import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../folder-services/auth.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  readonly form;

  constructor(private readonly fb: FormBuilder, private readonly authService: AuthService) {
    this.form = this.fb.nonNullable.group({
      currentPassword: ['', [Validators.required, Validators.minLength(6)]],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  submit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.getRawValue();
    if (payload.newPassword !== payload.confirmPassword) {
      this.errorMessage = 'Neues Passwort und Bestaetigung stimmen nicht ueberein.';
      this.successMessage = '';
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';
    this.isSubmitting = true;

    this.authService
      .changePassword(payload)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => {
          this.successMessage = 'Passwort wurde erfolgreich geaendert.';
          this.form.reset();
        },
        error: () => {
          this.errorMessage = 'Passwort konnte nicht geaendert werden.';
        }
      });
  }
}
