import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PreviewQueryComponent } from './preview-query.component';

describe('PreviewQueryComponent', () => {
  let component: PreviewQueryComponent;
  let fixture: ComponentFixture<PreviewQueryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PreviewQueryComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PreviewQueryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
