import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css'],
})
export class AuthComponent implements OnInit {
  loginForm!: FormGroup;
  registerForm!: FormGroup;

  constructor(
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
    });

    this.registerForm = new FormGroup({
      username: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
    });

    const logregBox = document.querySelector('.logreg-box') as HTMLElement;
    const loginLink = document.querySelector('.login-link') as HTMLElement;
    const registerLink = document.querySelector('.register-link') as HTMLElement;

    registerLink.addEventListener('click', () => {
      logregBox.classList.add('active');
    });

    loginLink.addEventListener('click', () => {
      logregBox.classList.remove('active');
    });
  }

  login(): void {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm?.value).subscribe();
    }
    console.log("The login button is being clicked");
  }

  register(): void {
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({});
    }
    console.log("The register button is being clicked");
  }
}
