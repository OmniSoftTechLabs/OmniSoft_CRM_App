import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { CalendarEvent, CalendarEventAction, CalendarEventTimesChangedEvent, CalendarView, } from 'angular-calendar';
import { isSameDay, isSameMonth } from 'date-fns';
import { addMinutes } from 'date-fns/fp';
import { Subject } from 'rxjs';
import { LeadFollowUpComponent } from '../lead-follow-up/lead-follow-up.component';
import { ManagerDash } from '../models/manager-dash';
import { RmanagerMaster } from '../models/rmanager-master';
import { UserMaster } from '../models/user-master';
import { AuthenticationService } from '../services/authentication.service';
import { AppoinmentStatus } from '../services/generic-enums';
import { LeadRepositoryService } from '../services/lead-repository.service';

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
  selector: 'app-admin-calender',
  templateUrl: './admin-calender.component.html',
  styleUrls: ['./admin-calender.component.css']
})
export class AdminCalenderComponent implements OnInit {

  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  refresh: Subject<any> = new Subject();
  managerDashboard: ManagerDash = new ManagerDash();
  filterUserList: RmanagerMaster[] = [];
  filteruserId: string = "0";
  isLoading: boolean = false;
  events: CalendarEvent[] = [];
  event: CalendarEvent;
  activeDayIsOpen: boolean = true;

  currentYear: number;
  currentMonth: string;

  constructor(private leadRepo: LeadRepositoryService, private auth: AuthenticationService, private datePipe: DatePipe, private router: Router
    , private modalService: NgbModal, private modalConfig: NgbModalConfig) {
    modalConfig.backdrop = 'static';
    modalConfig.keyboard = false;
    this.currentYear = new Date().getFullYear();
    this.currentMonth = datePipe.transform(new Date(), "MMM yyyy");
  }

  ngOnInit(): void {
    this.fillRManagerList();
  }

  fillRManagerList() {
    this.leadRepo.loadRManagerList().subscribe(
      rManager => {
        this.filterUserList = rManager;
      }, error => console.error(error)
    );
  }

  onSelectChange(value: any) {
    this.filteruserId = value.currentTarget.value;
    this.LoadData(this.filteruserId);
  }

  async LoadData(RMid) {
    this.isLoading = true;
    await this.leadRepo.loadCalenderByRM(RMid).then(
      data => {
        this.managerDashboard = data;
        this.events = [];
        this.managerDashboard.collCalendarEvents.forEach((item) => {

          this.events = [
            ...this.events,
            {
              id: item.callId,
              title: item.clientName + " - " + item.appointStatus + " - " + this.datePipe.transform(item.appointmentTime, "hh:mm a"),
              start: new Date(item.appointmentTime),
              end: addMinutes(30, new Date(item.appointmentTime)), // new Date(new Date(item.appointmentTime).setMinutes(new Date(item.appointmentTime).getMinutes() + 30)),
              color: item.appointStatusId == AppoinmentStatus.AppoinmentTaken ? colors.orange :
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
        this.isLoading = false;
      },
      error => { console.error(error); this.isLoading = false; }
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
      this.LoadData(this.filteruserId);
    });

  }

  onCardClose() {
    this.router.navigate(['/dashboard']);
  }

}
