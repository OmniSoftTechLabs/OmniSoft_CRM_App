import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbDatepickerI18n, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { TargetMatrix } from '../models/target-master';
import { GeneralRepositoryService } from '../services/general-repository.service';

@Component({
  selector: 'app-target-achieve-tele-caller',
  templateUrl: './target-achieve-tele-caller.component.html',
  styleUrls: ['./target-achieve-tele-caller.component.css']
})
export class TargetAchieveTeleCallerComponent implements OnInit {

  is_progress: boolean = false;
  targetMatrix: TargetMatrix = new TargetMatrix();
  selectedDate: NgbDateStruct;
  isWeek5visible: boolean = false;
  isWeek6visible: boolean = false;
  strPrintDate: string;
  totalTarget: number;
  totalAchieve: number;

  constructor(private router: Router, private generalRepository: GeneralRepositoryService, private datePipe: DatePipe, public i18n: NgbDatepickerI18n) {
    this.selectedDate = { day: 1, month: new Date().getMonth() + 1, year: new Date().getFullYear() };
    this.strPrintDate = new Date().toString();
  }

  ngOnInit(): void {
    this.onLoadTargetAchieve();
  }

  onLoadTargetAchieve() {
    this.is_progress = true;
    let date = new Date(this.selectedDate.year, this.selectedDate.month - 1, this.selectedDate.day, 0, 0, 0, 0);
    let strDate = this.datePipe.transform(date, "yyyy-MM-dd");

    this.generalRepository.GetTargetAchieveTelecaller(strDate).subscribe({
      next: data => {
        this.targetMatrix = data;

        this.is_progress = false;
        if (data.header.length == 8) {
          this.isWeek5visible = true;
          this.isWeek6visible = true;
        }
        else if (data.header.length == 7) {
          this.isWeek5visible = true;
          this.isWeek6visible = false;
        }
        else {
          this.isWeek5visible = false;
          this.isWeek6visible = false;
        }
      },
      error: error => (console.error = error.error, this.is_progress = false)
    });
  }

  onMonthChange() {
    this.onLoadTargetAchieve();
  }

  onCardClose() {
    this.router.navigate(['/dashboard']);
  }

  onPrint() {
    window.print();
  }

  ConvertToInt(val) {
    return parseInt(val);
  }
}
