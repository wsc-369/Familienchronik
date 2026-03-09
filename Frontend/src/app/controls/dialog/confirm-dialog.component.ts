import { CommonModule } from "@angular/common";
import { Component, signal } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap/modal";

@Component({
  selector: 'confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent {
  constructor(public activeModal: NgbActiveModal) {
  }
  //protected readonly title = signal('app_ahnenforschung-angular-frontend');

  save() {
    this.activeModal.close('save'); // gibt 'save' zurück
  }

  cancel() {
    this.activeModal.dismiss('cancel'); // gibt 'cancel' zurück
  }
}
