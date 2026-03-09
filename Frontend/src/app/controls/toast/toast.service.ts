import { Injectable, signal, TemplateRef } from '@angular/core';


export interface Toast {
	template: TemplateRef<any>;
	classname?: string;
	delay?: number;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
	private readonly _toasts = signal<Toast[]>([]);
	readonly toasts = this._toasts.asReadonly();

	// Globale Templates
	templates = {
		success: null as TemplateRef<any> | null,
		error: null as TemplateRef<any> | null,
		cancel: null as TemplateRef<any> | null,
		successSave: null as TemplateRef<any> | null,
		errorSave: null as TemplateRef<any> | null,
	};

	defaults = {
		success: { classname: 'bg-success text-light', delay: 3000 },
		error: { classname: 'bg-danger text-light', delay: 3000 },
		cancel: { classname: 'bg-warning text-dark', delay: 3000 },
		successSave: { classname: 'bg-success text-light', delay: 3000 },
		errorSave: { classname: 'bg-danger text-light', delay: 3000 }
	};

	registerTemplates(templates: {
		success: TemplateRef<any>;
		error: TemplateRef<any>;
		cancel: TemplateRef<any>;
		successSave: TemplateRef<any>;
		errorSave: TemplateRef<any>;
	}) {
		this.templates = templates;
	}

	show(toast: Toast) {
		toast.classname = toast.classname || this.defaults.success.classname;
		toast.delay = toast.delay || this.defaults.success.delay;
		this._toasts.update((toasts) => [...toasts, toast]);
	}

	cancel(toast: Toast) {
		toast.classname = toast.classname || this.defaults.cancel.classname;
		this._toasts.update((toasts) => [...toasts, toast]);
	}

	error(toast: Toast) {
		toast.classname = toast.classname || this.defaults.error.classname;
		this._toasts.update((toasts) => [...toasts, toast]);
	}

	successSave(toast: Toast) {
		toast.classname = toast.classname || this.defaults.successSave.classname;
		this._toasts.update((toasts) => [...toasts, toast]);
	}

	errorSave(toast: Toast) {
		toast.classname = toast.classname || this.defaults.errorSave.classname;
		this._toasts.update((toasts) => [...toasts, toast]);
	}

	remove(toast: Toast) {
		this._toasts.update((toasts) => toasts.filter((t) => t !== toast));
	}

	clear() {
		this._toasts.set([]);
	}
}