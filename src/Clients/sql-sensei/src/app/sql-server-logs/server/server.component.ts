import { Component, OnDestroy, OnInit, Signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  InsightsResponse,
  ServerCheckIssueCategory,
  ServerDetailsResponse,
  ServersApiService,
  SqlServerInsightsServerIssue,
  SqlServerPerformancePerformanceGraph,
  SqlServerPerformanceType,
  SqlServerPerformanceWaitStatGraph,
  SqlServerPerformanceWaitType,
} from '../servers-api.service';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { Subject, debounceTime, startWith, switchMap, takeUntil } from 'rxjs';
import { DatePipe } from '@angular/common';
import { DialogService } from '../dialog.service';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-server',
  templateUrl: './server.component.html',
  styleUrls: ['./server.component.css'],
})
export class ServerComponent implements OnInit, OnDestroy {
  id: number;
  server?: ServerDetailsResponse;
  insights?: InsightsResponse;

  waitStats?: SqlServerPerformanceWaitStatGraph[];
  performanceStats?: SqlServerPerformancePerformanceGraph[];

  ServerCheckIssueCategory = ServerCheckIssueCategory;

  start: Date;
  end: Date;

  dateMonitoring = new FormGroup({
    date: new FormControl<Date>(new Date(), { nonNullable: true }),
  });

  destroy$ = new Subject<void>();

  lineChartData: ChartConfiguration<'line'>['data'] = {
    datasets: [
      {
        data: [],
        label: 'CxPacket',
        fill: false,
        tension: 0.5,
        borderColor: 'black',
        backgroundColor: 'rgba(255,0,0,0.3)',
      },
      {
        data: [],
        label: 'SosSchedulerYield',
        fill: false,
        tension: 0.5,
        borderColor: 'blue',
        backgroundColor: 'rgba(0,0,255,0.3)',
      },
      {
        data: [],
        label: 'Threadpool',
        fill: false,
        tension: 0.5,
        borderColor: 'green',
        backgroundColor: 'rgba(0,255,0,0.3)',
      },
      {
        data: [],
        label: 'Lock',
        fill: false,
        tension: 0.5,
        borderColor: 'orange',
        backgroundColor: 'rgba(255,165,0,0.3)',
      },
      {
        data: [],
        label: 'ResourceSemaphore',
        fill: false,
        tension: 0.5,
        borderColor: 'purple',
        backgroundColor: 'rgba(128,0,128,0.3)',
      },
      {
        data: [],
        label: 'AsyncNetworkIo',
        fill: false,
        tension: 0.5,
        borderColor: 'brown',
        backgroundColor: 'rgba(165,42,42,0.3)',
      },
      {
        data: [],
        label: 'PageIoLatch',
        fill: false,
        tension: 0.5,
        borderColor: 'grey',
        backgroundColor: 'rgba(128,128,128,0.3)',
      },
      {
        data: [],
        label: 'WriteLog',
        fill: false,
        tension: 0.5,
        borderColor: 'yellow',
        backgroundColor: 'rgba(255,255,0,0.3)',
      },
    ],
  };

  lineChartPerformanceData: ChartConfiguration<'line'>['data'] = {
    datasets: [
      {
        data: [],
        label: 'Wait Time Per Core Per Second',
        fill: false,
        tension: 0.5,
        borderColor: 'blue',
        backgroundColor: 'rgba(0,0,255,0.3)',
      },
      {
        data: [],
        label: 'Re Compiles Per Second',
        fill: false,
        tension: 0.5,
        borderColor: 'green',
        backgroundColor: 'rgba(0,255,0,0.3)',
      },
      {
        data: [],
        label: 'Batch Requests Per Second',
        fill: false,
        tension: 0.5,
        borderColor: 'orange',
        backgroundColor: 'rgba(255,165,0,0.3)',
      },
    ],
  };

  lineChartPerformanceCpuData: ChartConfiguration<'line'>['data'] = {
    datasets: [
      {
        data: [],
        label: 'CPU Utilization',
        fill: false,
        tension: 0.5,
        borderColor: 'black',
        backgroundColor: 'rgba(255,0,0,0.3)',
      },
    ],
  };

  lineChartOptions: ChartOptions = {
    responsive: true,
    scales: {
      x: {
        display: false,
      },
      y: {
        display: true,
      },
    },
  };

  constructor(route: ActivatedRoute, private serversApi: ServersApiService, private datePipe: DatePipe, private dialog: DialogService) {
    this.id = Number.parseInt(route.snapshot.paramMap.get('id') ?? '0');

    const currentDate = new Date();
    const utcDate = new Date(
      Date.UTC(
        currentDate.getUTCFullYear(),
        currentDate.getUTCMonth(),
        currentDate.getUTCDate(),
        currentDate.getUTCHours(),
        currentDate.getUTCMinutes(),
        currentDate.getUTCSeconds()
      )
    );
    const utcDateSevenDaysBack = new Date(
      Date.UTC(
        currentDate.getUTCFullYear(),
        currentDate.getUTCMonth(),
        currentDate.getUTCDate(),
        currentDate.getUTCHours(),
        currentDate.getUTCMinutes(),
        currentDate.getUTCSeconds()
      )
    );
    utcDateSevenDaysBack.setUTCDate(utcDateSevenDaysBack.getUTCDate() - 7);

    this.start = utcDateSevenDaysBack;
    this.end = utcDate;
  }

  ngOnInit(): void {
    this.serversApi
      .getServer(this.id)
      .pipe(
        switchMap((server) => {
          this.server = server;

          return this.serversApi.getServerPerformanceStats(this.id, this.start, this.end);
        }),

        switchMap((performanceStats) => {
          this.lineChartPerformanceData.datasets.forEach((dataset) => {
            if (dataset.label === 'Wait Time Per Core Per Second') {
              dataset.data = performanceStats.filter((x) => x.type === SqlServerPerformanceType.WaitTimePerCorePerSec).map((x) => x.value);
            } else if (dataset.label === 'Re Compiles Per Second') {
              dataset.data = performanceStats.filter((x) => x.type === SqlServerPerformanceType.ReCompilesPerSecond).map((x) => x.value);
            } else if (dataset.label === 'Batch Requests Per Second') {
              dataset.data = performanceStats.filter((x) => x.type === SqlServerPerformanceType.BatchRequestsPerSecond).map((x) => x.value);
            }
          });

          this.lineChartPerformanceCpuData.datasets.forEach((dataset) => {
            if (dataset.label === 'CPU Utilization') {
              dataset.data = performanceStats.filter((x) => x.type === SqlServerPerformanceType.CpuUtilization).map((x) => x.value);
            }
          });

          this.lineChartPerformanceData.labels = performanceStats
            .filter((x) => x.type === SqlServerPerformanceType.WaitTimePerCorePerSec)
            .map((x) => this.datePipe.transform(x.dateTime, 'short'));
          this.lineChartPerformanceCpuData.labels = performanceStats
            .filter((x) => x.type === SqlServerPerformanceType.CpuUtilization)
            .map((x) => this.datePipe.transform(x.dateTime, 'short'));

          this.performanceStats = performanceStats;

          return this.serversApi.getServerWaitStats(this.id, this.start, this.end);
        }),
        takeUntil(this.destroy$)
      )
      .subscribe((waitStats) => {
        this.lineChartData.datasets.forEach((dataset) => {
          if (dataset.label === 'CxPacket') {
            dataset.data = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.CxPacket).map((x) => x.timeInMs);
          } else if (dataset.label === 'SosSchedulerYield') {
            dataset.data = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.SosSchedulerYield).map((x) => x.timeInMs);
          } else if (dataset.label === 'Threadpool') {
            dataset.data = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.Threadpool).map((x) => x.timeInMs);
          } else if (dataset.label === 'Lock') {
            dataset.data = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.Lock).map((x) => x.timeInMs);
          } else if (dataset.label === 'ResourceSemaphore') {
            dataset.data = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.ResourceSemaphore).map((x) => x.timeInMs);
          } else if (dataset.label === 'AsyncNetworkIo') {
            dataset.data = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.AsyncNetworkIo).map((x) => x.timeInMs);
          } else if (dataset.label === 'PageIoLatch') {
            dataset.data = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.PageIoLatch).map((x) => x.timeInMs);
          } else if (dataset.label === 'WriteLog') {
            dataset.data = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.WriteLog).map((x) => x.timeInMs);
          }
        });
        this.lineChartData.labels = waitStats.filter((x) => x.waitType === SqlServerPerformanceWaitType.CxPacket).map((x) => this.datePipe.transform(x.dateTime, 'short'));

        this.waitStats = waitStats;
      });

    this.dateMonitoring.controls.date.valueChanges
      .pipe(
        startWith(new Date()),
        debounceTime(500),
        switchMap((date) => this.serversApi.getServerInsights(this.id, date)),
        takeUntil(this.destroy$)
      )
      .subscribe((insights) => (this.insights = insights));
  }

  getBottleneck(): void {
    this.dialog.openWaitStatsExplanationDialog(this.waitStats);
  }

  convertHoursToDatetime(hoursAgo: number): Date {
    const dateNow = new Date();
    dateNow.setHours(hoursAgo * -1);

    return dateNow;
  }

  getServerChecksData(data: SqlServerInsightsServerIssue[], filter: ServerCheckIssueCategory): SqlServerInsightsServerIssue[] {
    return data.filter((x) => x.checkCategory === filter).sort((a, b) => a.priority - b.priority);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
