import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  public appName: string = 'SpamProtector';

  constructor(public authService: AuthService, private router: Router) { }

  ngOnInit(): void {
  }

  onLogoutClick(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  onHomeClick(): void {
    this.router.navigate(['/']);
  }

}
