import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { slideDownAnimation } from '../../shared/animation';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  animations: [slideDownAnimation],
})
export class HomeComponent {
  constructor(private authService: AuthService, private router: Router) {}

  navigateBasedOnAuthStatus(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/workout']);
    } else {
      this.router.navigate(['/auth']);
    }
  }
}
