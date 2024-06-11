import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  template: '<router-outlet></router-outlet>',
})
export class AppComponent implements OnInit, OnDestroy {
  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.authService.initializeAuth();

    if (this.authService.isLoggedIn()) {
      this.router.navigateByUrl('sql-server/servers');
    }
  }

  ngOnDestroy(): void {
    this.authService.onDestroy();
  }
}
