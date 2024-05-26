import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { ActivatedRoute, Router } from '@angular/router';
import { slideDownAnimation, slideDownWithBlurAnimation } from '../../shared/animation';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css'],
  animations: [slideDownAnimation, slideDownWithBlurAnimation],
})
export class AuthComponent implements OnInit {
  loginForm!: FormGroup;
  registerForm!: FormGroup;
  submitted: boolean = false;
  isRegisterActive: boolean = false;

  constructor(private authService: AuthService, private notificationService: NotificationService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.initForms();
    this.route.queryParams.subscribe((params) => {
      this.isRegisterActive = params['form'] === 'register';
    });
  }

  initForms(): void {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
      rememberMe: new FormControl(false),
    });

    this.registerForm = new FormGroup({
      username: new FormControl('', [Validators.required, Validators.minLength(3)]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    });
  }

  login(): void {
    this.submitted = true;
    if (this.loginForm.valid) {
      const { email, password, rememberMe } = this.loginForm.value;
      this.authService.login({ email, password }, rememberMe).subscribe({
        next: () => {
          this.notificationService.showSuccess('Login successful');
          this.router.navigate(['/home']);
        },
        error: (error) => {
          this.notificationService.showError('Login failed: ' + error.message);
        },
      });
    } else {
      this.checkFormErrors(this.loginForm);
    }
  }

  register(): void {
    this.submitted = true;
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          const { email, password } = this.registerForm.value;
          this.authService.login({ email, password }, false).subscribe({
            next: () => {
              this.notificationService.showSuccess('Registration and login successful');
              this.router.navigate(['/home']);
            },
            error: (error) => {
              this.notificationService.showError('Login after registration failed: ' + error.message);
            },
          });
        },
        error: (error) => {
          this.notificationService.showError('Registration failed: ' + error.message);
        },
      });
    } else {
      this.checkFormErrors(this.registerForm);
    }
  }
  

  toggleForms(): void {
    this.isRegisterActive = !this.isRegisterActive;
    this.loginForm.reset();
    this.registerForm.reset();
    this.submitted = false;
  }

  private checkFormErrors(form: FormGroup): void {
    for (const control in form.controls) {
      if (form.controls[control].errors) {
        for (const key in form.controls[control].errors) {
          if (key === 'required') {
            this.notificationService.showWarning(`${control.charAt(0).toUpperCase() + control.slice(1)} is required.`);
            return;
          } else if (key === 'email' && form.controls[control].hasError('email')) {
            this.notificationService.showWarning('Please enter a valid email address.');
            return;
          } else if (key === 'minlength') {
            const minLength = form.controls[control].errors?.['minlength'].requiredLength;
            this.notificationService.showWarning(`${control.charAt(0).toUpperCase() + control.slice(1)} must be at least ${minLength} characters long.`);
            return;
          }
        }
      }
    }
  }
}
