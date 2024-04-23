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
import { CreateNewServerDialogComponent } from './create-new-server-dialog/create-new-server-dialog.component';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { DeleteServerDialogComponent } from './delete-server-dialog/delete-server-dialog.component';

@NgModule({
  declarations: [ServersComponent, ServerComponent, WaitStatsExplanationDialogComponent, CreateNewServerDialogComponent, DeleteServerDialogComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
    MatChipsModule,
    MatSelectModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    MatGridListModule,
    BaseChartDirective,
    MatTabsModule,
    MatDialogModule,
    MatTableModule,
  ],
  providers: [provideCharts(withDefaultRegisterables()), DatePipe],
})
export class SqlServerLogsModule {}
