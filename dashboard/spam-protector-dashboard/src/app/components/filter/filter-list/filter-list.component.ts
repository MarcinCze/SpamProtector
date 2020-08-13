import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-filter-list',
  templateUrl: './filter-list.component.html',
  styleUrls: ['./filter-list.component.css']
})
export class FilterListComponent implements OnInit {
  @Input() rules;
  @Input() rulesType;

  public avatarClass: string;

  constructor() { }

  ngOnInit(): void {
    switch (this.rulesType) {
      case 'domains':
        this.avatarClass = 'domain';
        break;
      case 'subjects':
        this.avatarClass = 'subject';
        break;
      case 'senders':
        this.avatarClass = 'sender';
        break;
    }
  }
}
