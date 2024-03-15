import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { NgxEchartsDirective, provideEcharts } from "ngx-echarts";
import { EChartsOption } from 'echarts'

@Component({
  selector: 'app-pie-chart',
  standalone: true,
  imports: [
    NgxEchartsDirective
  ],
  providers: [
    provideEcharts(),
  ],
  template: `
    <div echarts [options]="options" class="echarts"></div>
  `
})
export class PieChartComponent implements OnChanges {
  @Input({ required: true }) data!: { name: string, value: number }[]
  @Input({ required: true }) title!: string
  @Input({ required: false }) titlePadding: number = 80

  options: EChartsOption = {};

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.updateOptions();
    }
  }

  private updateOptions(): void {
    this.options = {
      title: {
        text: this.title
      },
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b} : {c} ({d}%)',
      },
      legend: {
        orient: 'vertical',
        left: 'left',
        data: this.data.map(value => value.name),
        top: this.titlePadding,
      },
      series: [
        {
          name: 'Players',
          type: 'pie',
          radius: '80%',
          center: [ '50%', '50%' ],
          data: this.data,
        },
      ],
    };
  }
}
