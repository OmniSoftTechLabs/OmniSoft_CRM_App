import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CompanyMaster } from '../models/company-master';

@Component({
  selector: 'app-company-master',
  templateUrl: './company-master.component.html',
  styleUrls: ['./company-master.component.css']
})
export class CompanyMasterComponent implements OnInit {

  @ViewChild('companyAdd') form: NgForm;
  IsSucess: boolean = false;
  IsError: boolean = false;
  is_progress: boolean = false;
  errorMsg: string;
  successMsg: string;
  saveBtnTxt: string = "Save";
  companyModel: CompanyMaster = new CompanyMaster();

  constructor() { }

  ngOnInit(): void {
  }

  onSaveCompany() {

  }
}
