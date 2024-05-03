import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css'],
})
export class AuthComponent implements OnInit {
  loginForm!: FormGroup;
  registerForm!: FormGroup;
  submitted: boolean = false;

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(6),
      ]),
    });

    this.registerForm = new FormGroup({
      username: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(6),
      ]),
    });

    const logregBox = document.querySelector('.logreg-box') as HTMLElement;
    const loginLink = document.querySelector('.login-link') as HTMLElement;
    const registerLink = document.querySelector(
      '.register-link'
    ) as HTMLElement;

    registerLink.addEventListener('click', () => {
      logregBox.classList.add('active');
    });

    loginLink.addEventListener('click', () => {
      logregBox.classList.remove('active');
    });
  }

  login(): void {
    this.submitted = true;
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm?.value).subscribe(
        () => {
          this.notificationService.showSuccess('Login successful');
        },
        (error) => {
          this.notificationService.showError('Login failed: ' + error.message);
        }
      );
    } else {
      this.notificationService.showWarning(
        'Please fill in all required fields'
      );
    }
  }

  register(): void {
    this.submitted = true;
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe(
        () => {
          this.notificationService.showSuccess('Registration successful');
        },
        (error) => {
          this.notificationService.showError(
            'Registration failed: ' + error.message
          );
        }
      );
    } else {
      this.notificationService.showWarning(
        'Please fill in all required fields'
      );
    }
  }
}
