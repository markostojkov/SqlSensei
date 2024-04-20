import { Routes } from '@angular/router';
import { ServersComponent } from './servers/servers.component';
import { ServerComponent } from './server/server.component';

export const routes: Routes = [
  {
    component: ServersComponent,
    path: 'servers',
    pathMatch: 'full',
  },
  {
    component: ServerComponent,
    path: 'servers/:id',
  },
];
