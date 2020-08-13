import { Component, OnInit } from '@angular/core';
import { Filter } from 'src/app/models/filter';
import { FilterService } from 'src/app/services/filter.service';

@Component({
  selector: 'app-filter-subjects',
  templateUrl: './filter-subjects.component.html',
  styleUrls: ['./filter-subjects.component.css']
})
export class FilterSubjectsComponent implements OnInit {

  public subjects: Filter[];
  public type: string = 'subjects';

  constructor(private service: FilterService) { }

  ngOnInit(): void {
    this.service.getSubjects().subscribe(response => {
      this.subjects = response;
      console.log('Subjects: ', response);
    });
  }

}
