import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterSendersComponent } from './filter-senders.component';

describe('FilterSendersComponent', () => {
  let component: FilterSendersComponent;
  let fixture: ComponentFixture<FilterSendersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FilterSendersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FilterSendersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
