import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { BaseApiService } from '../shared/base-api.service';
import { Result } from '../shared/result.models';

@Injectable({
  providedIn: 'root',
})
export class ServersApiService {
  constructor(private baseApi: BaseApiService) {}

  getServers(): Observable<ServerResponse[]> {
    return this.baseApi.get<Result<ServerResponse[]>>('/sqlserver/v1/servers').pipe(map((x) => x.value));
  }

  createServer(req: CreateServerRequest): Observable<number> {
    return this.baseApi.post<Result<number>>('/sqlserver/v1/servers', req).pipe(map((x) => x.value));
  }

  getServer(id: number): Observable<ServerDetailsResponse> {
    return this.baseApi.get<Result<ServerDetailsResponse>>(`/sqlserver/v1/servers/${id}`).pipe(map((x) => x.value));
  }

  deleteServer(id: number): Observable<void> {
    return this.baseApi.delete<Result<void>>(`/sqlserver/v1/servers/${id}`).pipe(map((x) => x.value));
  }

  getServerWaitStats(id: number, start: Date, end: Date): Observable<SqlServerPerformanceWaitStatGraph[]> {
    return this.baseApi
      .get<Result<SqlServerPerformanceWaitStatGraph[]>>(`/sqlserver/v1/servers/${id}/wait-stats?start=${start.toISOString()}&end=${end.toISOString()}`)
      .pipe(map((x) => x.value));
  }

  getServerPerformanceStats(id: number, start: Date, end: Date): Observable<SqlServerPerformancePerformanceGraph[]> {
    return this.baseApi
      .get<Result<SqlServerPerformancePerformanceGraph[]>>(`/sqlserver/v1/servers/${id}/performance?start=${start.toISOString()}&end=${end.toISOString()}`)
      .pipe(map((x) => x.value));
  }
}

export class ServerResponse {
  constructor(public id: number, public name: string) {}
}

export class ServerDetailsResponse extends ServerResponse {
  constructor(
    id: number,
    name: string,
    public apiKey: string,
    public sqlServerCheck: SqlServerCheck,
    public sqlServerPerformanceCheck: SqlServerPerformanceCheck,
    public indexCheck: SqlServerIndexCheck
  ) {
    super(id, name);
  }
}

export enum ServerCheckIssueCategory {
  DataInDanger,
  ServerInDanger,
  ServerPerformance,
  ServerCacheClearAndWaits,
  ServerInfo,
  None,
}

export enum SqlServerPerformanceWaitType {
  CxPacket,
  SosSchedulerYield,
  Threadpool,
  Lock,
  ResourceSemaphore,
  AsyncNetworkIo,
  PageIoLatch,
  WriteLog,
}

export enum SqlServerPerformanceType {
  CpuUtilization,
  WaitTimePerCorePerSec,
  ReCompilesPerSecond,
  BatchRequestsPerSecond,
}

export interface SqlServerCheck {
  serverInfo: SqlServerInsightsServerInfo;
  cacheAndWaitStats: SqlServerInsightsCacheAndWaitStats;
  serverIssues: SqlServerInsightsServerIssue[];
}

export interface SqlServerPerformanceCheck {
  serverInfo: SqlServerPerformanceServerInfo;
  waitStats: SqlServerPerformanceWaitStat[];
  topBadQueries: SqlServerBadQuery[];
}

export interface SqlServerIndexCheck {
  removeIndices: SqlServerRemoveIndex[];
  addIndices: SqlServerAddIndex[];
}

export interface SqlServerInsightsServerIssue {
  priority: number;
  checkId: number;
  checkCategory: ServerCheckIssueCategory;
  details: string;
  scriptDetails: string;
  databaseName: string | null;
}

export interface SqlServerInsightsCacheAndWaitStats {
  waitsClearedRecently: boolean;
  cacheClearedRecently: boolean;
  noSignificantWaitStats: boolean;
  poisonWaits: boolean;
  poisonWaitType: string;
  poisonWaitsSerializableLocking: boolean;
}

export interface SqlServerInsightsServerInfo {
  is32Bit: boolean;
  lastRestartServer?: Date;
  lastRestartSqlServer?: Date;
  databaseCount?: number;
  databaseSizeInGb?: number;
  defaultTraceContentInHours?: number;
  logicalCpu?: number;
  memoryInGb?: number;
  serverName?: string;
  versionDetails?: string;
}

export interface SqlServerRemoveIndex {
  databaseName: string;
  tableName: string;
  index: string;
  message: string;
}

export interface SqlServerAddIndex {
  databaseName: string;
  tableName: string;
  magicBenefit: number;
  impact: string;
  indexDetails: string;
}

export interface SqlServerPerformanceServerInfo {
  batchRequestsPerSecond?: number;
  reCompilesPerSecond?: number;
  waitTimePerCorePerSec?: number;
  cpuUtilization?: number;
}

export interface SqlServerPerformanceWaitStat {
  timeInMs: number;
  waitType: SqlServerPerformanceWaitType;
}

export interface SqlServerPerformanceWaitStatGraph {
  timeInMs: number;
  waitType: SqlServerPerformanceWaitType;
  dateTime: Date;
}

export interface SqlServerPerformancePerformanceGraph {
  value: number | null;
  type: SqlServerPerformanceType;
  dateTime: Date;
}

export interface SqlServerBadQuery {
  id: number;
  databaseName?: string;
  executionsCount?: number;
  executionsPerMinuteCount?: number;
  totalCpu?: number;
  averageCpu?: number;
  totalDuration?: number;
  averageDuration?: number;
  totalReads?: number;
  averageReads?: number;
  totalReturnedRows?: number;
  averageReturnedRows?: number;
  numberOfPlans?: number;
  numberOfDistinctPlans?: number;
  lastExecutionTime?: Date;
  queryHash?: Uint8Array;
}

export enum SqlSenseiRunMonitoringPeriod {
  Every15Minutes = 0,
  Every30Minutes = 1,
  Every60Minutes = 2,
}

export enum SqlSenseiRunMaintenancePeriod {
  EveryWeekendSundayAt6AM = 0,
  EveryOtherWeekendSundayAt6AM = 1,
  Never = 2,
}

export class CreateServerRequest {
  constructor(public name: string, public monitoringPeriod: SqlSenseiRunMonitoringPeriod, public maintenancePeriod: SqlSenseiRunMaintenancePeriod) {}
}
