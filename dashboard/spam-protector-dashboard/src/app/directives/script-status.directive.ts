import { Directive, ElementRef, OnChanges, SimpleChanges, OnInit, Input } from '@angular/core';

@Directive({
  selector: '[appScriptStatus]'
})
export class ScriptStatusDirective implements OnInit, OnChanges {
  @Input('appScriptStatus') lastRun: string;

  constructor(private el: ElementRef) { }

  ngOnInit(): void {
    this.checkStatus();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.checkStatus();
  }

  private checkStatus(): void {
    console.log(this.lastRun);

    if (this.isActive()) {
      this.setGreen();
    }
    else {
      this.setRed();
    }
  }

  private isActive(): boolean {
    if (this.lastRun == null)
      return false;

    let date = new Date(this.lastRun);
    let now = new Date();

    return this.diffMinutes(now, date) <= 60;
  }

  private setRed(): void {
    this.setColor('#ff5757');
  }

  private setGreen(): void {
    this.setColor('#27ab4a');
  }

  private setColor(color: string) {
    this.el.nativeElement.style['background-color'] = color;
  }

  private diffMinutes(dt2, dt1) {
    var diff = (dt2.getTime() - dt1.getTime()) / 1000;
    diff /= 60;
    return Math.abs(Math.round(diff));
  }
}
