import { Component, OnInit } from '@angular/core';
import { RelaManagerStatusReport } from '../models/admin-report';
import { FilterOptions } from '../models/filter-options';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { LeadRepositoryService } from '../services/lead-repository.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-rele-manager-summary',
  templateUrl: './rele-manager-summary.component.html',
  styleUrls: ['./rele-manager-summary.component.css']
})
export class ReleManagerSummaryComponent implements OnInit {

  rmStatusReport: RelaManagerStatusReport = new RelaManagerStatusReport();
  filterOption: FilterOptions = new FilterOptions();
  fromDate: NgbDateStruct;
  toDate: NgbDateStruct;

  strFromDate: string;
  strToDate: string;
  strPrintDate: string;

  constructor(private leadRepo: LeadRepositoryService, private datePipe: DatePipe) {
    this.fromDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() };
    this.toDate = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() };
    this.strPrintDate = new Date().toString();
  }

  ngOnInit(): void {
    this.loadData();
  }

  async loadData() {
    this.strFromDate = this.datePipe.transform(new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day, 0, 0, 0, 0), "dd MMM yyyy");
    this.strToDate = this.datePipe.transform(new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day, 0, 0, 0, 0), "dd MMM yyyy");

    this.filterOption.fromDate = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
    this.filterOption.todate = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day);

    await this.leadRepo.getRMSummaryReport(this.filterOption).then(
      data => {
        data.rmRowsData.forEach(obj => {
          this.renameKey(obj, "firstMeeting", "First Meeting"),
            this.renameKey(obj, "secondMeeting", "Second Meeting"),
            this.renameKey(obj, "sold", "Sold"),
            this.renameKey(obj, "dropped", "Dropped"),
            this.renameKey(obj, "hold", "Work In Progress"),
            this.renameKey(obj, "notInterested", "Not Interested"),
            this.renameKey(obj, "rmName", "Relationship Manager"),
            this.renameKey(obj, "appointTaken", "Appointment Taken")
          this.renameKey(obj, "total", "Total")
        });
        this.rmStatusReport = data;

        //this.tcHeaders = this.tcStatusReport.header;
        //this.tcRows = this.tcStatusReport.tcRowsData;
      },
      error => console.error(error)
    );
  }

  renameKey(obj, oldKey, newKey) {
    obj[newKey] = obj[oldKey];
    delete obj[oldKey];
  }

  onDateChange() {
    this.loadData();
  }

  onPrint() {
    window.print();
  }

}
