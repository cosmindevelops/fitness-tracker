import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { slideDownAnimation } from '../../shared/animation';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  animations: [slideDownAnimation],
})
export class NavbarComponent {
  constructor(public authService: AuthService, private notificationService: NotificationService) {}

  logout(): void {
    this.authService.logout();
    this.notificationService.showSuccess('You have been logged out successfully.');
  }
}
