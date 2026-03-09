import { AsyncPipe, DecimalPipe } from '@angular/common';
import { Component, QueryList, Type, ViewChildren } from '@angular/core';
import { Observable } from 'rxjs';


import { FormsModule } from '@angular/forms';
import { NgbHighlight } from '@ng-bootstrap/ng-bootstrap/typeahead';
import { NgbPagination } from '@ng-bootstrap/ng-bootstrap/pagination';
import { UserService } from '../folder-services/user.service ';
import { NgbdSortableHeader, SortEvent } from '../folder-directives/sortable.directive';
import { User } from '../folder-interfaces/users';
import { NgbdModalConfirm, NgbdModalConfirmAutofocus, NgbdModalFocus } from './edit-User.component';
import { ConfirmDialogComponent } from '../controls/dialog/confirm-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap/modal';
import { ToastService } from '../controls/toast/toast.service';

const MODALS: { [name: string]: Type<any> } = {
	focusFirst: NgbdModalConfirm,
	autofocus: NgbdModalConfirmAutofocus,
};

@Component({
	selector: 'ngbd-table-complete',
	imports: [DecimalPipe, FormsModule, AsyncPipe, NgbHighlight, NgbdSortableHeader, NgbPagination],
	templateUrl: './applicationUser.component.html',
	providers: [UserService, DecimalPipe, NgbdModalFocus],
})
export class ApplicationUserComponent {
	users$: Observable<User[]>;
	total$: Observable<number>;

	@ViewChildren(NgbdSortableHeader) users!: QueryList<NgbdSortableHeader>;

	constructor(public service: UserService, private modalService: NgbModal, public toastService: ToastService) {
		this.users$ = service.countries$;
		this.total$ = service.total$;
	}

	onSort({ column, direction }: SortEvent) {
		// resetting other headers
		this.users.forEach((header) => {
			if (header.sortable !== column) {
				header.direction = '';
			}
		});

		this.service.sortColumn = column;
		this.service.sortDirection = direction;
	}

	onEdit(user: User) {
		const modalRef = this.modalService.open(ConfirmDialogComponent);

		// Optional: Daten an die Modal-Komponente weitergeben
		modalRef.componentInstance.user = user;

		// Ergebnis auswerten
		modalRef.result.then(
			(result) => {
				console.log('Modal geschlossen mit:', result);
				if (result === 'save') {
					this.toastService.show({
						template: this.toastService.templates.success!
					});

					this.toastService.successSave({
						template: this.toastService.templates.successSave!
					});

					this.toastService.errorSave({
						template: this.toastService.templates.errorSave!
					});
				}
				if (result === 'cancel') {
					this.toastService.cancel({
						template: this.toastService.templates.cancel!
					});
					/* this.toastService.show({
						template: this.toastService.templates.success!,
						classname: 'bg-warning text-light',
						delay: 3000
					}); */

				}
			},
			(reason) => {
				console.log('Modal abgebrochen mit:', reason);
				this.toastService.show({
					template: this.toastService.templates.cancel!,
					classname: 'bg-warning text-light',
					delay: 3000
				});
			}
		);
	}
}

