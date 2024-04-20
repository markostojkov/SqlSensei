import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WaitStatsExplanationDialogComponent } from './wait-stats-explanation-dialog.component';

describe('WaitStatsExplanationDialogComponent', () => {
  let component: WaitStatsExplanationDialogComponent;
  let fixture: ComponentFixture<WaitStatsExplanationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WaitStatsExplanationDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WaitStatsExplanationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
