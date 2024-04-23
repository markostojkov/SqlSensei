import { Component } from '@angular/core';
import { ServersApiService } from '../servers-api.service';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-delete-server-dialog',
  templateUrl: './delete-server-dialog.component.html',
  styleUrl: './delete-server-dialog.component.css',
})
export class DeleteServerDialogComponent {
  name?: string;
  id?: number;

  constructor(private apiService: ServersApiService, private dialogRef: MatDialogRef<DeleteServerDialogComponent>) {}

  submitForm(): void {
    if (this.id) {
      this.apiService.deleteServer(this.id).subscribe(() => this.dialogRef.close(true));
    }
  }
}
