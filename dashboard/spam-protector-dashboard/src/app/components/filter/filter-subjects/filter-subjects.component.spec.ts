import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterSubjectsComponent } from './filter-subjects.component';

describe('FilterSubjectsComponent', () => {
  let component: FilterSubjectsComponent;
  let fixture: ComponentFixture<FilterSubjectsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FilterSubjectsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FilterSubjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
