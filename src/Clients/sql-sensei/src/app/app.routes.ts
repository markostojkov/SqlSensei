import { Routes } from '@angular/router';
import { NavigationComponent } from './navigation/navigation.component';
import { MsalGuard, MsalRedirectComponent } from '@azure/msal-angular';

export const routes: Routes = [
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
];
