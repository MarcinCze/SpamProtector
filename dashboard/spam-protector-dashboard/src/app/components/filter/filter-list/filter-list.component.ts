import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-filter-list',
  templateUrl: './filter-list.component.html',
  styleUrls: ['./filter-list.component.css']
})
export class FilterListComponent implements OnInit {
  @Input() rules;
  @Input() rulesType;

  public avatarClass: string;

  constructor(private router: Router) { }

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

  onEditClick(id: number) {
    console.log('onEditClick: ', id);
    this.router.navigateByUrl(`/edit/${id}`);
  }

  onDeleteClick(id: number) {
    console.log('onDeleteClick: ', id);
  }
}
