import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
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

  constructor(private generalRepository: GeneralRepositoryService, private datePipe: DatePipe) {
    this.selectedDate = { day: 1, month: new Date().getMonth() + 1, year: new Date().getFullYear() };
  }

  ngOnInit(): void {
    this.onLoadTargetAchieve();
  }

  onLoadTargetAchieve() {
    this.is_progress = true;
    let date = new Date(this.selectedDate.year, this.selectedDate.month - 1, this.selectedDate.day, 0, 0, 0, 0);
    let strDate = this.datePipe.transform(date, "yyyy-MM-dd");

    this.generalRepository.GetTargetAchieveTelecaller(strDate).subscribe({
      next: data => (this.targetMatrix = data, this.is_progress = false),
      error: error => (console.error = error.error, this.is_progress = false)
    });
  }

}
