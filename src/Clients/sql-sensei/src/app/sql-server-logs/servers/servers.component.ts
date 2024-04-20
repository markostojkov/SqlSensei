import { Component } from '@angular/core';
import { ServersApiService } from '../servers-api.service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-servers',
  templateUrl: './servers.component.html',
  styleUrl: './servers.component.css',
})
export class ServersComponent {
  servers = toSignal(this.serversApi.getServers(), { initialValue: null });

  constructor(private serversApi: ServersApiService) {}
}
