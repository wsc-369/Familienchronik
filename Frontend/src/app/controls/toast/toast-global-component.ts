import { Component, inject, OnDestroy, TemplateRef } from '@angular/core';

import { ToastService } from './toast.service';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'ngbd-toast-global',
	imports: [CommonModule],
	templateUrl: './toast-global-component.html',
})
export class NgbdToastGlobal implements OnDestroy {
	toastService = inject(ToastService);

	showStandard(template: TemplateRef<any>) {
		this.toastService.show({ template });
	}

	showSuccess(template: TemplateRef<any>) {
		this.toastService.show({ template, classname: 'bg-success text-light', delay: 10000 });
	}

	showDanger(template: TemplateRef<any>) {
		this.toastService.show({ template, classname: 'bg-danger text-light', delay: 15000 });
	}

	ngOnDestroy(): void {
		this.toastService.clear();
	}
}