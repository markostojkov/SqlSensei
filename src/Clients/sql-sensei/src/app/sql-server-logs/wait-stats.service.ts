import { SqlServerPerformanceWaitStatGraph, SqlServerPerformanceWaitType } from './servers-api.service';

export class WaitStatsService {
  static getMaxWaitType(waitStats?: SqlServerPerformanceWaitStatGraph[]): MaxWaitType | null {
    if (!waitStats) {
      return null;
    }

    const cxPacketStats: SqlServerPerformanceWaitStatGraph[] = [];
    const sosSchedulerYieldStats: SqlServerPerformanceWaitStatGraph[] = [];
    const threadpoolStats: SqlServerPerformanceWaitStatGraph[] = [];
    const lockStats: SqlServerPerformanceWaitStatGraph[] = [];
    const resourceSemaphoreStats: SqlServerPerformanceWaitStatGraph[] = [];
    const asyncNetworkIoStats: SqlServerPerformanceWaitStatGraph[] = [];
    const pageIoLatchStats: SqlServerPerformanceWaitStatGraph[] = [];
    const writeLogStats: SqlServerPerformanceWaitStatGraph[] = [];

    for (const stat of waitStats) {
      switch (stat.waitType) {
        case SqlServerPerformanceWaitType.CxPacket:
          cxPacketStats.push(stat);
          break;
        case SqlServerPerformanceWaitType.SosSchedulerYield:
          sosSchedulerYieldStats.push(stat);
          break;
        case SqlServerPerformanceWaitType.Threadpool:
          threadpoolStats.push(stat);
          break;
        case SqlServerPerformanceWaitType.Lock:
          lockStats.push(stat);
          break;
        case SqlServerPerformanceWaitType.ResourceSemaphore:
          resourceSemaphoreStats.push(stat);
          break;
        case SqlServerPerformanceWaitType.AsyncNetworkIo:
          asyncNetworkIoStats.push(stat);
          break;
        case SqlServerPerformanceWaitType.PageIoLatch:
          pageIoLatchStats.push(stat);
          break;
        case SqlServerPerformanceWaitType.WriteLog:
          writeLogStats.push(stat);
          break;
      }
    }

    let maxWaitType: SqlServerPerformanceWaitType = SqlServerPerformanceWaitType.CxPacket;
    let maxSum = 0;

    const sumForWaitType = (stats: SqlServerPerformanceWaitStatGraph[]) => {
      return stats.reduce((acc, curr) => acc + curr.timeInMs, 0);
    };

    const updateMax = (waitType: SqlServerPerformanceWaitType, stats: SqlServerPerformanceWaitStatGraph[]) => {
      const sum = sumForWaitType(stats);
      if (sum > maxSum) {
        maxSum = sum;
        maxWaitType = waitType;
      }
    };

    updateMax(
      SqlServerPerformanceWaitType.CxPacket,
      waitStats.filter((stat) => stat.waitType === SqlServerPerformanceWaitType.CxPacket)
    );
    updateMax(
      SqlServerPerformanceWaitType.SosSchedulerYield,
      waitStats.filter((stat) => stat.waitType === SqlServerPerformanceWaitType.SosSchedulerYield)
    );
    updateMax(
      SqlServerPerformanceWaitType.Threadpool,
      waitStats.filter((stat) => stat.waitType === SqlServerPerformanceWaitType.Threadpool)
    );
    updateMax(
      SqlServerPerformanceWaitType.Lock,
      waitStats.filter((stat) => stat.waitType === SqlServerPerformanceWaitType.Lock)
    );
    updateMax(
      SqlServerPerformanceWaitType.ResourceSemaphore,
      waitStats.filter((stat) => stat.waitType === SqlServerPerformanceWaitType.ResourceSemaphore)
    );
    updateMax(
      SqlServerPerformanceWaitType.AsyncNetworkIo,
      waitStats.filter((stat) => stat.waitType === SqlServerPerformanceWaitType.AsyncNetworkIo)
    );
    updateMax(
      SqlServerPerformanceWaitType.PageIoLatch,
      waitStats.filter((stat) => stat.waitType === SqlServerPerformanceWaitType.PageIoLatch)
    );
    updateMax(
      SqlServerPerformanceWaitType.WriteLog,
      waitStats.filter((stat) => stat.waitType === SqlServerPerformanceWaitType.WriteLog)
    );

    return new MaxWaitType(maxWaitType, maxSum);
  }
}

export class MaxWaitType {
  public waitInS: number;

  constructor(public waitType: SqlServerPerformanceWaitType, public waitInMs: number) {
    this.waitInS = waitInMs / 1000;
  }
}
