import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  constructor(private router: Router) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.applyBlurEffect(event.url);
      }
    });
  }

  private applyBlurEffect(url: string): void {
    const urlTree = this.router.parseUrl(url);
    const formParam = urlTree.queryParams['form'];

    if (url.startsWith('/auth') && (!formParam || formParam === 'login' || formParam === 'register' || formParam === '')) {
      document.body.style.setProperty('--blur', '10px');
    } else {
      document.body.style.setProperty('--blur', '0');
    }
  }
}
