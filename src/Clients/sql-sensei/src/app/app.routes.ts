import { Routes } from '@angular/router';
import { NavigationComponent } from './navigation/navigation.component';
import { MsalGuard, MsalRedirectComponent } from '@azure/msal-angular';
import { LandingComponent } from './landing/landing.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'landing',
    pathMatch: 'full',
  },
  {
    path: 'landing',
    component: LandingComponent,
  },
  {
    path: 'auth',
    component: MsalRedirectComponent,
  },
  {
    path: '',
    component: NavigationComponent,
    canActivate: [MsalGuard],
    children: [
      {
        path: 'sql-server',
        loadChildren: async () => (await import('./sql-server-logs/sql-server-logs.module')).SqlServerLogsModule,
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'landing',
  },
];
