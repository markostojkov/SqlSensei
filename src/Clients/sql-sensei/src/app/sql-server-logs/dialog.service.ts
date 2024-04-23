import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { WaitStatsExplanationDialogComponent } from './wait-stats-explanation-dialog/wait-stats-explanation-dialog.component';
import { SqlServerPerformanceWaitStatGraph } from './servers-api.service';
import { CreateNewServerDialogComponent } from './create-new-server-dialog/create-new-server-dialog.component';
import { DeleteServerDialogComponent } from './delete-server-dialog/delete-server-dialog.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DialogService {
  constructor(private dialog: MatDialog) {}

  openWaitStatsExplanationDialog(waitStats?: SqlServerPerformanceWaitStatGraph[]): void {
    const ref = this.dialog.open(WaitStatsExplanationDialogComponent);

    ref.componentInstance.waitStats = waitStats;
  }

  createNewServer(): void {
    const ref = this.dialog.open(CreateNewServerDialogComponent);
  }

  deleteServer(name: string, id: number): Observable<boolean> {
    const ref = this.dialog.open(DeleteServerDialogComponent);

    ref.componentInstance.id = id;
    ref.componentInstance.name = name;

    return ref.afterClosed().pipe(map((x) => x === true));
  }
}
