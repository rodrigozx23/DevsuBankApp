import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { routes } from './app/app.routes';
import { ShellComponent } from './app/layout/shell.component';
import { provideZonelessChangeDetection } from '@angular/core';

bootstrapApplication(ShellComponent, {
  providers: [
    provideZonelessChangeDetection(), // <-- clave
    provideRouter(routes),
    provideHttpClient()
  ]
}).catch(err => console.error('BOOTSTRAP ERROR →', err));;
