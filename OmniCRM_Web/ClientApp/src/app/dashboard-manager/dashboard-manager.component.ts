import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Chart } from 'chart.js';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { AuthenticationService } from '../services/authentication.service';
import { UserMaster } from '../models/user-master';
import { ManagerDash } from '../models/manager-dash';
import { DatePipe } from '@angular/common';
import { Subject } from 'rxjs';
import { startOfDay, endOfDay, subDays, addDays, endOfMonth, isSameDay, isSameMonth, addHours, } from 'date-fns';
import { CalendarEvent, CalendarEventAction, CalendarEventTimesChangedEvent, CalendarView, } from 'angular-calendar';
import { addMinutes } from 'date-fns/fp';
import { GenericEnums, AppoinmentStatus } from '../services/generic-enums';
import { Router } from '@angular/router';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { LeadFollowUpComponent } from '../lead-follow-up/lead-follow-up.component';

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

  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  refresh: Subject<any> = new Subject();

  events: CalendarEvent[] = [];
  event: CalendarEvent;
  activeDayIsOpen: boolean = true;

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService, private datePipe: DatePipe, private router: Router
    , private modalService: NgbModal, private modalConfig: NgbModalConfig) {
    modalConfig.backdrop = 'static';
    modalConfig.keyboard = false;
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

        this.events = [];
        this.managerDashboard.collCalendarEvents.forEach((item) => {

          this.events = [
            ...this.events,
            {
              id: item.callId,
              title: item.clientName + " - " + item.appointStatus + " - " + this.datePipe.transform(item.appointmentTime, "hh:mm a"),
              start: new Date(item.appointmentTime),
              end: addMinutes(30, new Date(item.appointmentTime)), // new Date(new Date(item.appointmentTime).setMinutes(new Date(item.appointmentTime).getMinutes() + 30)),
              color: item.appointStatusId == AppoinmentStatus.Pending ? colors.orange :
                item.appointStatusId == AppoinmentStatus.FirstMeeting ? colors.purple :
                  item.appointStatusId == AppoinmentStatus.SecondMeeting ? colors.silver :
                    item.appointStatusId == AppoinmentStatus.Sold ? colors.green :
                      item.appointStatusId == AppoinmentStatus.Dropped ? colors.khaki :
                        item.appointStatusId == AppoinmentStatus.Hold ? colors.yellow :
                          item.appointStatusId == AppoinmentStatus.NotInterested ? colors.blue :
                            colors.blue,
            },
          ];
        });
      },
      error => console.error(error)
    );
  }

  closeOpenMonthViewDay() {
    this.activeDayIsOpen = false;
  }

  setView(view: CalendarView) {
    this.view = view;
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
      }
      this.viewDate = date;
    }
  }

  handleEvent(action: string, event: CalendarEvent): void {
    let callId = event.id;
    //this.router.navigate(['/lead-followup/' + callId]);

    const modalRef = this.modalService.open(LeadFollowUpComponent, { size: 'lg' });
    modalRef.componentInstance.callId = callId;
    modalRef.result.then((result) => {
      this.LoadData();
    });

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
    }, 500);


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
    }, 500);

  }
}
