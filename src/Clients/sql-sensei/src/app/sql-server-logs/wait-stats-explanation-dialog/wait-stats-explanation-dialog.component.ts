import { Component, Input, OnInit } from '@angular/core';
import { SqlServerPerformanceWaitStatGraph, SqlServerPerformanceWaitType } from '../servers-api.service';
import { MaxWaitType, WaitStatsService } from '../wait-stats.service';

@Component({
  selector: 'app-wait-stats-explanation-dialog',
  templateUrl: './wait-stats-explanation-dialog.component.html',
  styleUrls: ['./wait-stats-explanation-dialog.component.css'],
})
export class WaitStatsExplanationDialogComponent implements OnInit {
  @Input() waitStats?: SqlServerPerformanceWaitStatGraph[];

  SqlServerPerformanceWaitType = SqlServerPerformanceWaitType;
  biggestWaitStat?: MaxWaitType | null;

  ngOnInit(): void {
    if (this.waitStats && this.waitStats.length > 0) {
      this.biggestWaitStat = WaitStatsService.getMaxWaitType(this.waitStats);
    }
  }
}
