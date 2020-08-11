import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterDomainsComponent } from './filter-domains.component';

describe('FilterDomainsComponent', () => {
  let component: FilterDomainsComponent;
  let fixture: ComponentFixture<FilterDomainsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FilterDomainsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FilterDomainsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
