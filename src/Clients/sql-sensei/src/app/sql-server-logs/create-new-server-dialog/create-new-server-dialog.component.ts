import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SqlSenseiRunMonitoringPeriod, SqlSenseiRunMaintenancePeriod, ServersApiService, CreateServerRequest } from '../servers-api.service';
import { Router } from '@angular/router';

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
    monitoringPeriod: new FormControl(SqlSenseiRunMonitoringPeriod.Every15Minutes, { validators: [Validators.required], nonNullable: true }),
    maintenancePeriod: new FormControl(SqlSenseiRunMaintenancePeriod.EveryWeekendSundayAt6AM, { validators: [Validators.required], nonNullable: true }),
  });

  monitoringPeriodEnum = [
    new PeriodKeyValue('Every15Minutes', SqlSenseiRunMonitoringPeriod.Every15Minutes),
    new PeriodKeyValue('Every30Minutes', SqlSenseiRunMonitoringPeriod.Every30Minutes),
    new PeriodKeyValue('Every60Minutes', SqlSenseiRunMonitoringPeriod.Every60Minutes),
  ];
  maintenancePeriodEnum = [
    new PeriodKeyValue('EveryWeekendSundayAt6AM', SqlSenseiRunMaintenancePeriod.EveryWeekendSundayAt6AM),
    new PeriodKeyValue('EveryOtherWeekendSundayAt6AM', SqlSenseiRunMaintenancePeriod.EveryOtherWeekendSundayAt6AM),
    new PeriodKeyValue('Never', SqlSenseiRunMaintenancePeriod.Never),
  ];

  constructor(private apiService: ServersApiService, private router: Router) {}

  submitForm(): void {
    const req = new CreateServerRequest(this.formGroup.controls.serverName.value, this.formGroup.controls.monitoringPeriod.value, this.formGroup.controls.maintenancePeriod.value);

    this.apiService.createServer(req).subscribe((serverId) => {
      const currentUrl = this.router.url;
      const serverUrl = `${currentUrl}/${serverId}`;
      this.router.navigateByUrl(serverUrl);
    });
  }
}
