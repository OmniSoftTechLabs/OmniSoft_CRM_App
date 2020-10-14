import { Component, OnInit } from '@angular/core';
import { RowsData } from '../models/admin-report';

@Component({
  selector: 'app-tele-caller-summary',
  templateUrl: './tele-caller-summary.component.html',
  styleUrls: ['./tele-caller-summary.component.css']
})
export class TeleCallerSummaryComponent implements OnInit {

  tcHeaders: string[] = [];
  tcRows: RowsData[] = [];

  constructor() { }

  ngOnInit(): void {
  }

}
