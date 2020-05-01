import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Chart } from 'chart.js';
import { TeleDash } from '../models/tele-dash';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';


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

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit(): void {
    this.loadData();
  }

  async loadData() {
    await this.leadRepo.loadTeleDash(this.currentUser.userId).then(
      data => { this.teleDashboard = data; },
      error => console.error(error)
    );
  }

  ngAfterViewInit() {

    var salesChartCanvas = this.canvas.nativeElement.getContext('2d');

    var salesChartData = {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
      datasets: [
        {
          label: 'Digital Goods',
          backgroundColor: 'rgba(60,141,188,0.9)',
          borderColor: 'rgba(60,141,188,0.8)',
          //pointRadius: false,
          pointColor: '#3b8bba',
          pointStrokeColor: 'rgba(60,141,188,1)',
          pointHighlightFill: '#fff',
          pointHighlightStroke: 'rgba(60,141,188,1)',
          data: [28, 48, 40, 19, 86, 27, 90, 67, 74, 63, 89, 47]
        },
        {
          label: 'Electronics',
          backgroundColor: 'rgba(210, 214, 222, 2)',
          borderColor: 'rgba(210, 214, 222, 1)',
          //pointRadius: false,
          pointColor: 'rgba(210, 214, 222, 1)',
          pointStrokeColor: '#c1c7d1',
          pointHighlightFill: '#fff',
          pointHighlightStroke: 'rgba(220,220,220,1)',
          data: [65, 59, 80, 81, 56, 55, 40, 53, 71, 123, 47, 56]
        },
      ]
    }

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
    }


    // This will get the first returned node in the jQuery collection.
    this.chart = new Chart(salesChartCanvas, {
      type: 'line',
      data: salesChartData,
      options: salesChartOptions
    }
    )

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
