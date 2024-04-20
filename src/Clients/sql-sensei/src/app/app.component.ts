import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  template: '<router-outlet></router-outlet>',
})
export class AppComponent implements OnInit, OnDestroy {
  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.initializeAuth();
  }

  ngOnDestroy(): void {
    this.authService.onDestroy();
  }
}
