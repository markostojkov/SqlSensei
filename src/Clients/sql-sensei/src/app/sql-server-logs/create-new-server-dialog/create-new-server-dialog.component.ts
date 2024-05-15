import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SqlSenseiRunMonitoringPeriod, SqlSenseiRunMaintenancePeriod, ServersApiService, CreateServerRequest } from '../servers-api.service';
import { Router } from '@angular/router';
import { MatDialogRef } from '@angular/material/dialog';

class PeriodKeyValue<T> {
  constructor(public key: string, public value: T) {}
}

@Component({
  selector: 'app-create-new-server-dialog',
  templateUrl: './create-new-server-dialog.component.html',
  styleUrls: ['./create-new-server-dialog.component.css'],
})
export class CreateNewServerDialogComponent {
  formGroup = new FormGroup({
    serverName: new FormControl('', { validators: [Validators.required, Validators.maxLength(64)], nonNullable: true }),
    monitoringPeriod: new FormControl(SqlSenseiRunMonitoringPeriod.Every2Hours, { validators: [Validators.required], nonNullable: true }),
    maintenancePeriod: new FormControl(SqlSenseiRunMaintenancePeriod.EveryWeekendSundayAt6AM, { validators: [Validators.required], nonNullable: true }),
  });

  monitoringPeriodEnum = [
    new PeriodKeyValue('Every 2 Hours', SqlSenseiRunMonitoringPeriod.Every2Hours),
    new PeriodKeyValue('Every 4 Hours', SqlSenseiRunMonitoringPeriod.Every4Hours),
    new PeriodKeyValue('Every 6 Hours', SqlSenseiRunMonitoringPeriod.Every6Hours),
  ];
  maintenancePeriodEnum = [
    new PeriodKeyValue('Every Weekend Sunday At 6AM', SqlSenseiRunMaintenancePeriod.EveryWeekendSundayAt6AM),
    new PeriodKeyValue('Every Other Weekend Sunday At 6AM', SqlSenseiRunMaintenancePeriod.EveryOtherWeekendSundayAt6AM),
    new PeriodKeyValue('Never', SqlSenseiRunMaintenancePeriod.Never),
  ];

  constructor(private apiService: ServersApiService, private router: Router, private dialog: MatDialogRef<CreateNewServerDialogComponent>) {}

  submitForm(): void {
    const req = new CreateServerRequest(this.formGroup.controls.serverName.value, this.formGroup.controls.monitoringPeriod.value, this.formGroup.controls.maintenancePeriod.value);

    this.apiService.createServer(req).subscribe((serverId) => {
      const currentUrl = this.router.url;
      const serverUrl = `${currentUrl}/${serverId}`;
      this.router.navigateByUrl(serverUrl);

      this.dialog.close();
    });
  }
}
