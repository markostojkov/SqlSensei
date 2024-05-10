import { Component, OnInit } from '@angular/core';
import { MaintenanceResponse, ServerDetailsResponse, ServersApiService } from '../servers-api.service';
import { ActivatedRoute } from '@angular/router';
import { debounceTime, startWith, switchMap } from 'rxjs';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-server-maintenance',
  templateUrl: './server-maintenance.component.html',
  styleUrls: ['./server-maintenance.component.css'],
})
export class ServerMaintenanceComponent implements OnInit {
  id: number;
  server?: ServerDetailsResponse;
  maintenance: MaintenanceResponse[] = [];

  dateMonitoring = new FormGroup({
    date: new FormControl<Date>(new Date(), { nonNullable: true }),
  });

  constructor(private serversApi: ServersApiService, route: ActivatedRoute) {
    this.id = Number.parseInt(route.snapshot.paramMap.get('id') ?? '0');
  }

  ngOnInit(): void {
    this.serversApi.getServer(this.id).subscribe((server) => (this.server = server));

    this.dateMonitoring.controls.date.valueChanges
      .pipe(
        startWith(new Date()),
        debounceTime(500),
        switchMap((date) => this.serversApi.getServerMaintenance(this.id, date))
      )
      .subscribe((maintenance) => {
        this.maintenance = maintenance;
      });
  }
}
