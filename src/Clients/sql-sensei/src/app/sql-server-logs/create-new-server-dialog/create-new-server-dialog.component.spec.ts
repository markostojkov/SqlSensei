import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateNewServerDialogComponent } from './create-new-server-dialog.component';

describe('CreateNewServerDialogComponent', () => {
  let component: CreateNewServerDialogComponent;
  let fixture: ComponentFixture<CreateNewServerDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateNewServerDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreateNewServerDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
