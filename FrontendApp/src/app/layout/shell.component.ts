import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './header.component';
import { SidebarComponent } from './sidebar.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, SidebarComponent],
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.css']
})

export class ShellComponent {}