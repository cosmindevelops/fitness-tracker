import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (event.url === '/auth?form=login' || event.url === '/auth?form=register') {
          document.body.style.setProperty('--blur', '10px');
        } else {
          document.body.style.setProperty('--blur', '0');
        }
      }
    });
  }
}