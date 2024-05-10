import { Routes } from '@angular/router';
import { ServersComponent } from './servers/servers.component';
import { ServerComponent } from './server/server.component';
import { ServerMaintenanceComponent } from './server-maintenance/server-maintenance.component';

export const routes: Routes = [
  {
    component: ServersComponent,
    path: 'servers',
    pathMatch: 'full',
  },
  {
    component: ServerMaintenanceComponent,
    path: 'servers/:id/maintenance',
  },
  {
    component: ServerComponent,
    path: 'servers/:id',
  },
];
