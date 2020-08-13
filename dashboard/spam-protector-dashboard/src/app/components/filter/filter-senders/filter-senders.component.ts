import { Component, OnInit } from '@angular/core';
import { FilterService } from '../../../services/filter.service';
import { Filter } from 'src/app/models/filter';

@Component({
  selector: 'app-filter-senders',
  templateUrl: './filter-senders.component.html',
  styleUrls: ['./filter-senders.component.css']
})
export class FilterSendersComponent implements OnInit {

  public senders: Filter[];
  public type: string = 'senders';

  constructor(private service: FilterService) { }

  ngOnInit(): void {
    this.service.getSenders().subscribe(response => {
      this.senders = response;
      console.log('Senders: ', response);
    });
  }

  onDeleteClick(id: number): void {
    console.log('Rule delete clicked', id);

    this.service.delete(id).subscribe(response => {
      console.log(response);
      this.service.getSenders().subscribe(response => {
        this.senders = response;
        console.log('Senders: ', response);
      });
    });
  }
}
