import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Chart } from 'chart.js';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { AuthenticationService } from '../services/authentication.service';
import { UserMaster } from '../models/user-master';
import { ManagerDash } from '../models/manager-dash';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-dashboard-manager',
  templateUrl: './dashboard-manager.component.html',
  styleUrls: ['./dashboard-manager.component.css']
})
export class DashboardManagerComponent implements OnInit {

  @ViewChild('areaChart') areaChart: ElementRef;
  @ViewChild('donutChart') donutChart: ElementRef;
  currentUser: UserMaster;
  managerDashboard: ManagerDash = new ManagerDash();
  months: string[] = [];
  sold: number[] = [];
  dropped: number[] = [];
  currentYear: number;
  currentMonth: string;

  monthlyDonutData: number[] = [];


  areaData = [];
  donutData = [];

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService, private datePipe: DatePipe) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.currentYear = new Date().getFullYear();
    this.currentMonth = datePipe.transform(new Date(), "MMM yyyy");
  }

  ngOnInit(): void {
    this.LoadData();
  }

  async LoadData() {
    await this.leadRepo.loadManagerDash(this.currentUser.userId).then(
      data => {
        this.managerDashboard = data;
        this.monthlyDonutData.push(data.monthlyDropped);
        this.monthlyDonutData.push(data.monthlySold);
        this.monthlyDonutData.push(data.monthlyHold);

        this.managerDashboard.collChartData.forEach((item) => {
          this.months.push(item.month);
          this.sold.push(item.sold);
          this.dropped.push(item.dropped);
        });

      },
      error => console.error(error)
    );
  }

  ngAfterViewInit() {
    var revenueChartCanvas = this.areaChart.nativeElement.getContext('2d');
    
    var salesChartData = {
      labels: this.months,
      datasets: [
        {
          label: 'Sold',
          backgroundColor: 'rgba(88, 214, 141, 0.2)',
          borderColor: 'rgba(88, 214, 141, 1)',
          data: this.sold
        },
        {
          label: 'Dropped',
          backgroundColor: 'rgba(236, 112, 99, 0.2)',
          borderColor: 'rgba(236, 112, 99, 1)',
          data: this.dropped
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
      this.areaData = new Chart(revenueChartCanvas, {
        type: 'line',
        data: salesChartData,
        options: salesChartOptions
      });
    }, 300);


    // Donut Chart
    var pieChartCanvas = this.donutChart.nativeElement.getContext('2d');
    var pieData = {
      labels: [
        'Dropped Leads',
        'Sold Leads',
        'Hold Leads',
      ],
      datasets: [
        {
          data: this.monthlyDonutData,
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

    setTimeout(() => {
      this.donutData = new Chart(pieChartCanvas, {
        type: 'doughnut',
        data: pieData,
        options: pieOptions
      });
    }, 300);

  }
}
