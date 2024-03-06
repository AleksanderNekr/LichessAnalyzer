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
  @Input({ required: true }) xAxisData!: string[];
  @Input({ required: true }) yAxisData!: number[];

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
        data: this.xAxisData,
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          name: "Rating",
          data: this.yAxisData,
          type: 'line',
        },
      ],
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b}: {c}',
      },
    };
  }
}
