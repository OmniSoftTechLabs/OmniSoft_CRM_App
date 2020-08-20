import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Chart } from 'chart.js';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { AdminDash } from '../models/admin-dash';
import { FilterOptions } from '../models/filter-options';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

const colors: any = {
  green: {
    primary: '#32CD32',
    secondary: '#99E699',
  },
  blue: {
    primary: '#4E95DD',
    secondary: '#A7CAEE',
  },
  yellow: {
    primary: '#FFD700',
    secondary: '#FFEB80',
  },
  purple: {
    primary: '#AD8CF0',
    secondary: '#D6C6F7',
  },
  orange: {
    primary: '#FA8072',
    secondary: '#FCC0B9',
  },
  silver: {
    primary: '#C0C0C0',
    secondary: '#E0E0E0'
  },
  khaki: {
    primary: '#BDB76B',
    secondary: '#DEDBB5'
  }
};

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  @ViewChild('canvas') canvas: ElementRef;
  chart: any;
  adminDashboard: AdminDash = new AdminDash();
  teleCallers: string[] = [];
  noResponse: number[] = [];
  notInterested: number[] = [];
  appoinmentTaken: number[] = [];
  callLater: number[] = [];
  wrongNumber: number[] = [];
  none: number[] = [];
  filterOption: FilterOptions = new FilterOptions();
  fromDate: NgbDateStruct;
  toDate: NgbDateStruct;


  constructor(private leadRepo: LeadRepositoryService) {
    this.fromDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() };
    this.toDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() };
  }

  ngOnInit(): void {
    this.loadData();
  }

  async loadData() {
    this.filterOption.fromDate = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
    this.filterOption.todate = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day);

    await this.leadRepo.loadAdminDash(this.filterOption).then(
      data => {
        this.adminDashboard = data;

        this.adminDashboard.collAdminChartData.forEach((item) => {
          this.teleCallers.push(item.telecaller);
          this.noResponse.push(item.noResponse);
          this.notInterested.push(item.notInterested);
          this.appoinmentTaken.push(item.appoinmentTaken);
          this.callLater.push(item.callLater);
          this.wrongNumber.push(item.wrongNumber);
          this.none.push(item.none);
        });
      },
      error => console.error(error)
    );
  }

  ngAfterViewInit(): void {
    this.loadChart();
  }

  loadChart() {
    var stackedBarChartCanvas = this.canvas.nativeElement.getContext('2d');
    var stackedBarChartData = {
      labels: this.teleCallers,
      datasets: [
        {
          label: 'No Response',
          backgroundColor: colors.yellow.secondary, //'rgba(88, 214, 141, 0.2)',
          borderColor: colors.yellow.primary,//'rgba(88, 214, 141, 1)',
          data: this.noResponse
        },
        {
          label: 'Not Interested',
          backgroundColor: colors.orange.secondary,
          borderColor: colors.orange.primary,
          data: this.notInterested
        },
        {
          label: 'Appoint. Taken',
          backgroundColor: colors.green.secondary,
          borderColor: colors.green.primary,
          data: this.appoinmentTaken
        },
        {
          label: 'Call Later',
          backgroundColor: colors.khaki.secondary,
          borderColor: colors.khaki.primary,
          data: this.callLater
        },
        {
          label: 'Wrong Number',
          backgroundColor: colors.purple.secondary,
          borderColor: colors.purple.primary,
          data: this.wrongNumber
        },
        {
          label: 'None',
          backgroundColor: colors.silver.secondary,
          borderColor: colors.silver.primary,
          data: this.none
        },
      ]
    };

    var stackedBarChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        xAxes: [{
          stacked: true,
        }],
        yAxes: [{
          stacked: true
        }]
      }
    }

    //var areaChartData = {
    //  labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
    //  datasets: [
    //    {
    //      label: 'Digital Goods',
    //      backgroundColor: 'rgba(60,141,188,0.9)',
    //      borderColor: 'rgba(60,141,188,0.8)',
    //      pointRadius: false,
    //      pointColor: '#3b8bba',
    //      pointStrokeColor: 'rgba(60,141,188,1)',
    //      pointHighlightFill: '#fff',
    //      pointHighlightStroke: 'rgba(60,141,188,1)',
    //      data: [28, 48, 40, 19, 86, 27, 90]
    //    },
    //    {
    //      label: 'Electronics',
    //      backgroundColor: 'rgba(210, 214, 222, 1)',
    //      borderColor: 'rgba(210, 214, 222, 1)',
    //      pointRadius: false,
    //      pointColor: 'rgba(210, 214, 222, 1)',
    //      pointStrokeColor: '#c1c7d1',
    //      pointHighlightFill: '#fff',
    //      pointHighlightStroke: 'rgba(220,220,220,1)',
    //      data: [65, 59, 80, 81, 56, 55, 40]
    //    },
    //  ]
    //}

    setTimeout(() => {
      this.chart = new Chart(stackedBarChartCanvas, {
        type: 'bar',
        data: stackedBarChartData,
        options: stackedBarChartOptions
      });
    }, 500);
  }

  onDateChange() {
    this.teleCallers = [];
    this.noResponse = [];
    this.notInterested = [];
    this.appoinmentTaken = [];
    this.callLater = [];
    this.wrongNumber = [];
    this.none = [];
    this.chart.destroy();

    this.loadData();
    this.loadChart();
  }
}
