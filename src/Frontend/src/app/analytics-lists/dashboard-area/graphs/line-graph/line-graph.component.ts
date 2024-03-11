import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { NgxEchartsDirective, provideEcharts } from "ngx-echarts";
import { EChartsOption } from 'echarts'

@Component({
  selector: 'app-line-graph',
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
export class LineGraphComponent implements OnChanges {
  @Input({ required: true }) xAxisData!: Date[]
  @Input({ required: true }) yAxisData!: { 'name': string, data: number[], 'type': 'line' }[]

  options: EChartsOption = {};

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['xAxisData'] || changes['yAxisData']) {
      this.updateOptions();
    }
  }

  private updateOptions(): void {
    this.options = {
      xAxis: {
        type: 'category',
        data: this.xAxisData.map(date => date.toDateString()),
      },
      yAxis: {
        type: 'value',
      },
      series: this.yAxisData,
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b}: {c}',
      },
      legend: {
        name: "Legend",
        left: 80
      },
      title: {
        text: "Ratings"
      },
    };
  }
}
