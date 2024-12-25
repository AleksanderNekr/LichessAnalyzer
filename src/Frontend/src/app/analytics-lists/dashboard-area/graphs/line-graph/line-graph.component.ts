import { Component, Input, OnChanges, SimpleChanges } from "@angular/core";
import { NgxEchartsDirective, provideEcharts } from "ngx-echarts";
import { EChartsOption } from "echarts";

@Component({
  selector: "app-line-graph",
  standalone: true,
  imports: [
    NgxEchartsDirective,
  ],
  providers: [
    provideEcharts(),
  ],
  template: `
    <div echarts [options]="options" class="echarts"></div>
  `,
})
export class LineGraphComponent implements OnChanges {
  @Input({ required: true }) xAxisData!: Date[];
  @Input({ required: true }) yAxisData!: { name: string; data: { name: Date, value: number }[]; type: "line" }[];
  @Input({ required: true }) title!: string;
  @Input({ required: false }) titlePadding: number = 80;

  options: EChartsOption = {};

  ngOnChanges(changes: SimpleChanges): void {
    if (changes["xAxisData"] || changes["yAxisData"]) {
      this.updateOptions();
    }
  }

  private updateOptions(): void {
    // Ensure data alignment
    if (!this.xAxisData || !this.yAxisData || !this.yAxisData.length)
      return;

    this.options = {
      xAxis: {
        type: "category",
        data: this.xAxisData.map(date => date.toISOString().split("T")[0]), // Format dates as "YYYY-MM-DD"
      },
      yAxis: {
        type: "value",
      },
      series: this.yAxisData.map(({ name, data, type }) => ({
        name,
        type,
        data: data.map(({ name, value }) => [ name.toISOString().split("T")[0], value ]), // Format dates as
                                                                                          // "YYYY-MM-DD"
      })),
      tooltip: {
        trigger: "item",
        formatter: "{a} <br/>{c}",
      },
      legend: {
        name: "Legend",
        left: this.titlePadding,
      },
      title: {
        text: this.title,
      },
    };
  }
}
