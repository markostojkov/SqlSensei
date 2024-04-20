import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { routes } from './sql-server-logs.routes';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ServersComponent } from './servers/servers.component';
import { ServerComponent } from './server/server.component';
import { MatGridListModule } from '@angular/material/grid-list';
import { BaseChartDirective, provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDialogModule } from '@angular/material/dialog';
import { WaitStatsExplanationDialogComponent } from './wait-stats-explanation-dialog/wait-stats-explanation-dialog.component';

@NgModule({
  declarations: [ServersComponent, ServerComponent, WaitStatsExplanationDialogComponent],
  imports: [CommonModule, RouterModule.forChild(routes), MatCardModule, MatButtonModule, MatTooltipModule, MatGridListModule, BaseChartDirective, MatTabsModule, MatDialogModule],
  providers: [provideCharts(withDefaultRegisterables()), DatePipe],
})
export class SqlServerLogsModule {}
