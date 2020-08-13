import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'ruleValue'
})
export class RuleValuePipe implements PipeTransform {

  private maxLength: number = 25;

  transform(value: string): string {

    if (value.length > this.maxLength) {
      value = value.substring(0, this.maxLength - 3) + '...';
    }

    return value;
  }

}
