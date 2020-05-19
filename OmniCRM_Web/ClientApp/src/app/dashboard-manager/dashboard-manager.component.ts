import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-dashboard-manager',
  templateUrl: './dashboard-manager.component.html',
  styleUrls: ['./dashboard-manager.component.css']
})
export class DashboardManagerComponent implements OnInit {

  @ViewChild('revenueChart') revenueChart: ElementRef;
  @ViewChild('salesChart') salesChart: ElementRef;

  revenueData = [];
  salesData = [];

  constructor() { }

  ngOnInit(): void {
  }

  ngAfterViewInit() {
    var revenueChartCanvas = this.revenueChart.nativeElement.getContext('2d');

    var salesChartData = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
        {
          label: 'Digital Goods',
          backgroundColor: 'rgba(60,141,188,0.9)',
          borderColor: 'rgba(60,141,188,0.8)',
          pointRadius: false,
          pointColor: '#3b8bba',
          pointStrokeColor: 'rgba(60,141,188,1)',
          pointHighlightFill: '#fff',
          pointHighlightStroke: 'rgba(60,141,188,1)',
          data: [28, 48, 40, 19, 86, 27, 90]
        },
        {
          label: 'Electronics',
          backgroundColor: 'rgba(210, 214, 222, 1)',
          borderColor: 'rgba(210, 214, 222, 1)',
          pointRadius: false,
          pointColor: 'rgba(210, 214, 222, 1)',
          pointStrokeColor: '#c1c7d1',
          pointHighlightFill: '#fff',
          pointHighlightStroke: 'rgba(220,220,220,1)',
          data: [65, 59, 80, 81, 56, 55, 40]
        },
      ]
    };

    var salesChartOptions = {
      maintainAspectRatio: false,
      responsive: true,
      legend: {
        display: false
      },
      scales: {
        xAxes: [{
          gridLines: {
            display: false,
          }
        }],
        yAxes: [{
          gridLines: {
            display: false,
          }
        }]
      }
    };

    // This will get the first returned node in the jQuery collection.
    this.revenueData = new Chart(revenueChartCanvas, {
      type: 'line',
      data: salesChartData,
      options: salesChartOptions
    });



    // Donut Chart
    var pieChartCanvas = this.salesChart.nativeElement.getContext('2d');
    var pieData = {
      labels: [
        'Instore Sales',
        'Download Sales',
        'Mail-Order Sales',
      ],
      datasets: [
        {
          data: [30, 12, 20],
          backgroundColor: ['#f56954', '#00a65a', '#f39c12'],
        }
      ]
    }
    var pieOptions = {
      legend: {
        display: false
      },
      maintainAspectRatio: false,
      responsive: true,
    }
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    this.salesData = new Chart(pieChartCanvas, {
      type: 'doughnut',
      data: pieData,
      options: pieOptions
    });


  }
}
