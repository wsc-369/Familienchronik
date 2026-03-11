import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'; 
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { jwtInterceptor } from './folder-services/jwt.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    NgbModule,
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([jwtInterceptor]))
  ]
};
