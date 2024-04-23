import { Component } from '@angular/core';
import { ServersApiService } from '../servers-api.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { DialogService } from '../dialog.service';
import { BehaviorSubject, Subject, switchMap, takeUntil } from 'rxjs';

@Component({
  selector: 'app-servers',
  templateUrl: './servers.component.html',
  styleUrl: './servers.component.css',
})
export class ServersComponent {
  private refreshTrigger$ = new BehaviorSubject<void>(undefined);
  private destroy$ = new Subject<void>();

  servers = toSignal(this.refreshTrigger$.pipe(switchMap(() => this.serversApi.getServers())), { initialValue: null });

  constructor(private serversApi: ServersApiService, private dialog: DialogService) {}

  createNewServer(): void {
    this.dialog.createNewServer();
  }

  refreshServers(): void {
    this.refreshTrigger$.next();
  }

  deleteServer(name: string, id: number): void {
    this.dialog
      .deleteServer(name, id)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.refreshServers();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
