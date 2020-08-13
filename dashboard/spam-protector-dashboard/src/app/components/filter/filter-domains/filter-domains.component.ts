import { Component, OnInit } from '@angular/core';
import { Filter } from 'src/app/models/filter';
import { FilterService } from 'src/app/services/filter.service';

@Component({
  selector: 'app-filter-domains',
  templateUrl: './filter-domains.component.html',
  styleUrls: ['./filter-domains.component.css']
})
export class FilterDomainsComponent implements OnInit {

  public domains: Filter[];
  public type: string = 'domains';

  constructor(private service: FilterService) { }

  ngOnInit(): void {
    this.service.getDomains().subscribe(response => {
      this.domains = response;
      console.log('Domains: ', response);
    });
  }

}
