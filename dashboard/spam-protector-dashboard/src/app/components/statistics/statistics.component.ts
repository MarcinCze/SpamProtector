import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy, OnChanges, SimpleChanges } from '@angular/core';
import { FilterService } from 'src/app/services/filter.service';
import { Stats } from 'src/app/models/stats';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css'],
})
export class StatisticsComponent implements OnInit, OnChanges {

  stats: Stats;

  constructor(private filterService: FilterService) { }

  ngOnInit(): void {
    this.filterService.getStats().subscribe(response => {
      console.log(response);
      this.stats = response;
    });
  }

  ngOnChanges(changes: SimpleChanges): void { }

}
