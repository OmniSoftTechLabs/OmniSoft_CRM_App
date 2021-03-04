import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbDatepickerNavigateEvent, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
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

  constructor(private router: Router, private generalRepository: GeneralRepositoryService) {
    this.selectedDate = { day: 1, month: new Date().getMonth() + 1, year: new Date().getFullYear() };
  }

  ngOnInit(): void {

    this.is_progress = true;
    var event = new Date();
    let date = JSON.stringify(event)
    date = date.slice(1, 11)

    this.generalRepository.getTargetEntry(date).subscribe({
      next: data => (this.targetEntryList = data, this.is_progress = false),
      error: error => (console.error = error.error, this.IsError = true, this.is_progress = false)
    });

    //this.generalRepository.getTargetEntry(new Date()).subscribe({
    //  next: data => (this.successMsg = "Company Created Successfully..", this.IsSucess = true, this.onSaveCompleted()),
    //  error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
    //});
  }

  onSaveCompleted() {
    setTimeout(() => { this.closeAlert(); }, 5000);
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }

  onSave(targetId: number) {
    this.isEdit = false;
  }

  onEdit(targetId: number) {
    this.isEdit = true;
    this.editTargetId = targetId;
  }

  onMonthChange() {
    var abd = this.selectedDate;
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }

  onCardClose() {
    this.router.navigate(['/dashboard']);
  }
}
