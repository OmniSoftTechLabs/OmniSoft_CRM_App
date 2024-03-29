import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbDate, NgbDatepickerI18n, NgbDatepickerNavigateEvent, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { Data } from 'popper.js';
import { TargetMaster, TargetMatrix } from '../models/target-master';
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
  targetMatrix: TargetMatrix = new TargetMatrix();
  selectedDate: NgbDateStruct;
  isEdit: boolean = false;
  editTargetId: number = 0;
  isWeek5visible: boolean = false;
  isWeek6visible: boolean = false;
  minDate: NgbDateStruct;
  maxDate: NgbDateStruct;
  isBackDate: boolean = false;
  constructor(private router: Router, private generalRepository: GeneralRepositoryService, private datePipe: DatePipe, public i18n: NgbDatepickerI18n) {
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

    //this.generalRepository.getTargetEntry(strDate).subscribe({
    //  next: data => (this.targetEntryList = data, this.is_progress = false),
    //  error: error => (console.error = error.error, this.IsError = true, this.is_progress = false)
    //});

    this.generalRepository.GetTargetMatrix(strDate).subscribe({
      next: data => {
        this.targetMatrix = data;
        this.is_progress = false;
        if (data.header.length == 7) {
          this.isWeek5visible = true;
          this.isWeek6visible = true;
        }
        else if (data.header.length == 6) {
          this.isWeek5visible = true;
          this.isWeek6visible = false;
        }
        else {
          this.isWeek5visible = false;
          this.isWeek6visible = false;
        }
      },
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

    this.generalRepository.postTargetMatrix(strDate, this.targetMatrix).subscribe({
      next: data => (console.log('Success!', data), this.onLoadTarget()),
      error: error => (console.error('Error!', error), this.onLoadTarget())
    });

    //this.generalRepository.postTargetEntry(strDate, this.targetEntryList).subscribe({
    //  next: data => (console.log('Success!', data), this.onLoadTarget()),
    //  error: error => (console.error('Error!', error), this.onLoadTarget())
    //});

  }

  onEdit(targetId: number) {
    this.isEdit = true;
    this.editTargetId = targetId;
  }

  onMonthChange() {
    this.isEdit = false;

    let seleDate = new Date(this.selectedDate.year, this.selectedDate.month - 1, this.selectedDate.day, 0, 0, 0, 0);
    let currDate = new Date();
    if (seleDate.getMonth() < currDate.getMonth() || seleDate.getFullYear() < currDate.getFullYear()) {
      this.isBackDate = true;
    }
    else {
      this.isBackDate = false;
    }
    this.onLoadTarget();
  }

  onTextChange(event: any, tcId: any) {
    let target = event.target.value;
    this.targetMatrix.rowDataTargetMaster.forEach(obj => {
      if (obj.telecallerId == tcId) {
        if (obj.targetWeek2 == 0)
          obj.targetWeek2 = target;
        if (obj.targetWeek3 == 0)
          obj.targetWeek3 = target;
        if (obj.targetWeek4 == 0)
          obj.targetWeek4 = target;
        if (obj.targetWeek5 == 0 && this.isWeek5visible == true)
          obj.targetWeek5 = target;
        if (obj.targetWeek6 == 0 && this.isWeek6visible == true)
          obj.targetWeek6 = target;
      }
    });
  }

  renameKey(obj, oldKey, newKey) {
    obj[newKey] = obj[oldKey];
    delete obj[oldKey];
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }

  onCardClose() {
    this.router.navigate(['/dashboard']);
  }
}
