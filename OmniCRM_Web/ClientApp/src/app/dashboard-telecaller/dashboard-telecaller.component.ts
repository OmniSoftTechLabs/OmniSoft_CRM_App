import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Chart } from 'chart.js';
import { TeleDash } from '../models/tele-dash';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { delay } from 'rxjs/operators';


@Component({
  selector: 'app-dashboard-telecaller',
  templateUrl: './dashboard-telecaller.component.html',
  styleUrls: ['./dashboard-telecaller.component.css']
})
export class DashboardTelecallerComponent implements OnInit {

  @ViewChild('canvas') canvas: ElementRef;
  chart = [];
  teleDashboard: TeleDash = new TeleDash();
  currentUser: UserMaster;
  months: string[] = [];
  appoinTaken: number[] = [];
  notInterest: number[] = [];
  currentYear: number;

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.currentYear = new Date().getFullYear();
  }

  ngOnInit(): void {
    this.loadData();
  }

  async loadData() {
    await this.leadRepo.loadTeleDash(this.currentUser.userId).then(
      data => {
        this.teleDashboard = data;

        this.teleDashboard.collChartData.forEach((item) => {
          this.months.push(item.month);
          this.appoinTaken.push(item.appoinTaken);
          this.notInterest.push(item.notInterest);
        });

      },
      error => console.error(error)
    );
  }

  ngAfterViewInit() {
    var salesChartCanvas = this.canvas.nativeElement.getContext('2d');

    var salesChartData = {
      //labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
      labels: this.months,
      datasets: [
        {
          label: 'Appoin. Taken',
          backgroundColor: 'rgba(88, 214, 141, 0.2)',
          borderColor: 'rgba(88, 214, 141, 1)',
          //pointRadius: false,
          //pointColor: '#3b8bba',
          //pointStrokeColor: 'rgba(60,141,188,1)',
          //pointHighlightFill: '#fff',
          //pointHighlightStroke: 'rgba(60,141,188,1)',
          //data: [55, 88, 66, 74, 75, 25, 12, 36, 0]
          data: this.appoinTaken
        },
        {
          label: 'Not Interest',
          backgroundColor: 'rgba(236, 112, 99, 0.2)',
          borderColor: 'rgba(236, 112, 99, 1)',
          //pointRadius: false,
          //pointColor: 'rgba(210, 214, 222, 1)',
          //pointStrokeColor: '#c1c7d1',
          //pointHighlightFill: '#fff',
          //pointHighlightStroke: 'rgba(220,220,220,1)',
          //data: [32, 41, 75, 68, 14, 85, 34, 26, 0]
          data: this.notInterest
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

    setTimeout(() => {
      this.chart = new Chart(salesChartCanvas, {
        type: 'line',
        data: salesChartData,
        options: salesChartOptions
      });
    }, 300);




    //this.chart = new Chart(this.canvas.nativeElement.getContext('2d'), {
    //  type: 'line',
    //  data: {
    //    labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'], // your labels array
    //    datasets: [
    //      {
    //        data: [65, 59, 80, 81, 56, 55, 40], // your data array
    //        borderColor: '#00AEFF',
    //        fill: false
    //      }
    //    ]
    //  },
    //  options: {
    //    maintainAspectRatio: false,
    //    legend: {
    //      display: false
    //    },
    //    scales: {
    //      xAxes: [{
    //        display: true
    //      }],
    //      yAxes: [{
    //        display: true
    //      }],
    //    }
    //  }
    //});

  }
}
