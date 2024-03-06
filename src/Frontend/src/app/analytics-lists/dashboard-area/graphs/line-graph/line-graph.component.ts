import { Component } from '@angular/core';
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
export class LineGraphComponent {
  protected options: EChartsOption = {
    xAxis: {
      type: 'category',
      data: [ 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun' ],
    },
    yAxis: {
      type: 'value',
    },
    series: [
      {
        data: [ 820, 932, 901, 934, 1290, 1330, 1320 ],
        type: 'line',
      },
    ],
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b} : {c}',
    },
  }
}
