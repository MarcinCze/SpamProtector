import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../../confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-filter-list',
  templateUrl: './filter-list.component.html',
  styleUrls: ['./filter-list.component.css']
})
export class FilterListComponent implements OnInit {
  @Input() rules;
  @Input() rulesType;
  @Output() clickDelete = new EventEmitter<number>();

  public avatarClass: string;

  constructor(private router: Router, public dialog: MatDialog) { }

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
    const dialogRef = this.dialog.open(ConfirmationDialogComponent);

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
      if (result === true) {
        this.clickDelete.emit(id);
      }
    });
  }
}
