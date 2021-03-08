import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbDate, NgbDatepickerNavigateEvent, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { Data } from 'popper.js';
import { TargetMaster } from '../models/target-master';
import { GeneralRepositoryService } from '../services/general-repository.service';

declare var $: any;

@Component({
  selector: 'app-target-entry',
  templateUrl: './target-entry.component.html',
  styleUrls: ['./target-entry.component.css']
})
export class TargetEntryComponent implements OnInit {

  //@ViewChild('labelImport') labelImport: ElementRef;



  IsSucess: boolean = false;
  IsError: boolean = false;
  is_progress: boolean = false;
  errorMsg: string;
  successMsg: string;
  saveBtnTxt: string = "Save";
  targetEntryList: TargetMaster[] = [];
  selectedDate: NgbDateStruct;
  isEdit: boolean = false;
  editTargetId: number = 0;

  minDate: NgbDateStruct;
  maxDate: NgbDateStruct;
  isBackDate: boolean = false;
  constructor(private router: Router, private generalRepository: GeneralRepositoryService, private datePipe: DatePipe) {
    this.selectedDate = { day: 1, month: new Date().getMonth() + 1, year: new Date().getFullYear() };
    //this.firstDateofMonths.push();
  }

  ngOnInit(): void {
    this.onLoadTarget();
  }

  onLoadTarget() {
    this.is_progress = true;
    let date = new Date(this.selectedDate.year, this.selectedDate.month - 1, this.selectedDate.day, 0, 0, 0, 0);
    let strDate = this.datePipe.transform(date, "yyyy-MM-dd");

    this.generalRepository.getTargetEntry(strDate).subscribe({
      next: data => (this.targetEntryList = data, this.is_progress = false),
      error: error => (console.error = error.error, this.IsError = true, this.is_progress = false)
    });
  }

  onSaveCompleted() {
    setTimeout(() => { this.closeAlert(); }, 5000);
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }

  onSave(targetId: number) {
    this.isEdit = false;

    this.is_progress = true;
    let date = new Date(this.selectedDate.year, this.selectedDate.month - 1, this.selectedDate.day, 0, 0, 0, 0);
    let strDate = this.datePipe.transform(date, "yyyy-MM-dd");

    this.generalRepository.postTargetEntry(strDate, this.targetEntryList).subscribe({
      next: data => (console.log('Success!', data), this.onLoadTarget()),
      error: error => (console.error('Error!', error), this.onLoadTarget())
    });

  }

  onEdit(targetId: number) {
    this.isEdit = true;
    this.editTargetId = targetId;
  }

  onMonthChange() {
    this.isEdit = false;

    let seleDate = new Date(this.selectedDate.year, this.selectedDate.month - 1, this.selectedDate.day, 0, 0, 0, 0);
    let currDate = new Date();
    if (seleDate.getMonth() < currDate.getMonth()) {
      this.isBackDate = true;
    }
    else {
      this.isBackDate = false;
    }
    this.onLoadTarget();
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }

  onCardClose() {
    this.router.navigate(['/dashboard']);
  }
}
