import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.maxLength(50)])
  });

  get email() { return this.loginForm.get('email'); }
  get password() { return this.loginForm.get('password'); }

  constructor(private auth: AuthService, private router: Router) { }

  ngOnInit(): void {
  }

  onLoginSubmit() {
    console.log("On login");
    this.auth.login(this.loginForm.value.email, this.loginForm.value.password);

    if (this.auth.isAuthenticated) {
      this.router.navigate(['/home']);
    }
  }
}
