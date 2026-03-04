import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';


// Hier importierst du die gewünschten ngx-bootstrap Module
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { ModalModule } from 'ngx-bootstrap/modal';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent // deine eigenen Komponenten
  ],
  imports: [
    BrowserModule,
    ButtonsModule.forRoot(), // Registrierung von ngx-bootstrap Buttons
    ModalModule.forRoot()    // Registrierung von ngx-bootstrap Modals
  ],
  providers: [],
  bootstrap: [AppComponent] // Startkomponente
})
export class AppModule {}
