import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { WaitStatsExplanationDialogComponent } from './wait-stats-explanation-dialog/wait-stats-explanation-dialog.component';
import { SqlServerPerformanceWaitStatGraph } from './servers-api.service';

@Injectable({
  providedIn: 'root',
})
export class DialogService {
  constructor(private dialog: MatDialog) {}

  openWaitStatsExplanationDialog(waitStats?: SqlServerPerformanceWaitStatGraph[]): void {
    const ref = this.dialog.open(WaitStatsExplanationDialogComponent);

    ref.componentInstance.waitStats = waitStats;
  }
}
