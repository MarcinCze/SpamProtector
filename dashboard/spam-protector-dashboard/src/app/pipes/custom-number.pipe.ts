import { Pipe, PipeTransform } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Pipe({
  name: 'customNumber'
})
export class CustomNumberPipe implements PipeTransform {

  transform(value: number): string {
    if (value != null) {
      let decimalPipe = new DecimalPipe('en-GB');
      let result = decimalPipe.transform(value, '5.0-0');
      return result.replace(',', ' ');
    }
    return "0";
  }

}
