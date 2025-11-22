import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app'; // Corregido de App a AppComponent

bootstrapApplication(AppComponent, appConfig) // Corregido de App a AppComponent
  .catch((err) => console.error(err));
