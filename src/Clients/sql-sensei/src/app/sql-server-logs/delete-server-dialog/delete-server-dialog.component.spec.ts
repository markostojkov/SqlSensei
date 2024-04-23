import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteServerDialogComponent } from './delete-server-dialog.component';

describe('DeleteServerDialogComponent', () => {
  let component: DeleteServerDialogComponent;
  let fixture: ComponentFixture<DeleteServerDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteServerDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeleteServerDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
