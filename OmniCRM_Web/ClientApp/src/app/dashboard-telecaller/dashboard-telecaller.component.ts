import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Chart } from 'chart.js';
import { TeleDash } from '../models/tele-dash';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { delay } from 'rxjs/operators';
import { DatePipe } from '@angular/common';


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
  currentMonth: string;
  percentTotal: number;
  percentNoResponse: number;
  percentAppoTaken: number;
  percentNotInter: number;

  percentLastMonthTotal: number;
  percentLastMonthNoResponse: number;
  percentLastMonthAppoTaken: number;
  percentLastMonthNotInter: number;

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService, private datePipe: DatePipe) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.currentYear = new Date().getFullYear();
    this.currentMonth = datePipe.transform(new Date(), "MMM yyyy");
  }

  ngOnInit(): void {
    this.loadData();
  }

  async loadData() {
    await this.leadRepo.loadTeleDash(this.currentUser.userId).then(
      data => {
        this.teleDashboard = data;

        this.percentTotal = (this.teleDashboard.monthlyTotalLeads / 200) * 100;
        this.percentNoResponse = (this.teleDashboard.monthlyNoResponse / this.teleDashboard.monthlyTotalLeads) * 100;
        this.percentAppoTaken = (this.teleDashboard.monthlyAppoinmentTaken / this.teleDashboard.monthlyTotalLeads) * 100;
        this.percentNotInter = (this.teleDashboard.monthlyNotInterested / this.teleDashboard.monthlyTotalLeads) * 100;

        this.percentLastMonthTotal = (this.teleDashboard.monthlyTotalLeads - this.teleDashboard.lastMonthTotalLeads) * 100 / this.teleDashboard.lastMonthTotalLeads;
        this.percentLastMonthNoResponse = (this.teleDashboard.monthlyNoResponse - this.teleDashboard.lastMonthNoResponse) * 100 / this.teleDashboard.lastMonthNoResponse;
        this.percentLastMonthAppoTaken = (this.teleDashboard.monthlyAppoinmentTaken - this.teleDashboard.lastMonthAppoinmentTaken) * 100 / this.teleDashboard.lastMonthAppoinmentTaken;
        this.percentLastMonthNotInter = (this.teleDashboard.monthlyNotInterested - this.teleDashboard.lastMonthNotInterested) * 100 / this.teleDashboard.lastMonthNotInterested;

        this.teleDashboard.collChartData.forEach((item) => {
          this.months.push(item.month);
          this.appoinTaken.push(item.appoinTaken);
          this.notInterest.push(item.notInterest);
        });

      },
      error => console.error(error)
    );
  }

  ngAfterViewInit(): void {
    var salesChartCanvas = this.canvas.nativeElement.getContext('2d');

    var salesChartData = {
      labels: this.months,
      datasets: [
        {
          label: 'Appoin. Taken',
          backgroundColor: 'rgba(88, 214, 141, 0.2)',
          borderColor: 'rgba(88, 214, 141, 1)',
          data: this.appoinTaken
        },
        {
          label: 'Not Interest',
          backgroundColor: 'rgba(236, 112, 99, 0.2)',
          borderColor: 'rgba(236, 112, 99, 1)',
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

    setTimeout(() => {
      this.chart = new Chart(salesChartCanvas, {
        type: 'line',
        data: salesChartData,
        options: salesChartOptions
      });
    }, 1600);
  }
}
