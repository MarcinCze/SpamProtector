import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private lsTokenKey = 'SpamProtector_Token';

  constructor() { }

  login(login: string, password: string): void {
    console.warn('AuthService.login: fake method');
    const token = 'abcdef123456!@#$%';
    this.cleanLocalStorage();
    localStorage.setItem(this.lsTokenKey, token);
  }

  logout(): void {
    console.warn('AuthService.logout: fake method');
    this.cleanLocalStorage();
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem(this.lsTokenKey);
    return token != null;
  }

  getToken(): string {
    return localStorage.getItem(this.lsTokenKey);
  }

  private cleanLocalStorage(): void {
    localStorage.removeItem(this.lsTokenKey);
  }
}
