import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { slideDownAnimation } from '../../shared/animation';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  animations: [slideDownAnimation],
})
export class HomeComponent implements OnInit{

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  navigateBasedOnAuthStatus(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/workout']);
    } else {
      this.router.navigate(['/auth']);
    }
  }

  ngOnInit(): void {
    this.handleGoogleLoginCallback();
  }

  private handleGoogleLoginCallback(): void {
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      if (token) {
        this.saveToken(token, true);
      }
    });
  }

  private saveToken(token: string, rememberMe: boolean): void {
    if (rememberMe) {
      localStorage.setItem('access_token', token);
    } else {
      sessionStorage.setItem('access_token', token);
    }
  }
}
