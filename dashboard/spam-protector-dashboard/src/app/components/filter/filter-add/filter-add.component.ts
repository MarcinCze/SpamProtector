import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FilterService } from 'src/app/services/filter.service';
import { Filter } from 'src/app/models/filter';

@Component({
  selector: 'app-filter-add',
  templateUrl: './filter-add.component.html',
  styleUrls: ['./filter-add.component.css']
})
export class FilterAddComponent implements OnInit {

  ruleType: {}[] = [
    { value: 'domain', viewValue: 'Domain' },
    { value: 'sender', viewValue: 'Sender' },
    { value: 'subject', viewValue: 'Subject' }
  ];

  ruleForm = new FormGroup({
    value: new FormControl('', [Validators.required]),
    type: new FormControl('', [Validators.required]),
    isActive: new FormControl({ value: true, disabled: true }, [Validators.required])
  });

  get value() { return this.ruleForm.get('value'); }
  get type() { return this.ruleForm.get('type'); }
  get isActive() { return this.ruleForm.get('isActive'); }

  constructor(private router: Router, private filterService: FilterService) { }

  ngOnInit(): void {
  }

  onRuleSubmit(): void {
    console.log('onRuleSubmit');

    let rule = new Filter();
    rule.type = this.ruleForm.value.type;
    rule.value = this.ruleForm.value.value;
    console.log('New rule: ', rule);

    this.filterService.add(rule).subscribe(response => {
      console.log(response);
      this.router.navigateByUrl('/home');
    })
  }

  onCancelClick(): void {
    this.router.navigateByUrl('/home');
  }
}
