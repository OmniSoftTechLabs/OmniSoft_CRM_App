import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CompanyMaster } from '../models/company-master';
import { UserMaster } from '../models/user-master';
import { GeneralRepositoryService } from '../services/general-repository.service';
import { roles } from '../services/generic-enums';

class ImageSnippet {
  pending: boolean = false;
  status: string = 'init';

  constructor(public src: string, public file: File) { }
}

@Component({
  selector: 'app-company-master',
  templateUrl: './company-master.component.html',
  styleUrls: ['./company-master.component.css']
})
export class CompanyMasterComponent implements OnInit {

  @ViewChild('companyAdd') form: NgForm;
  @ViewChild('labelImport') labelImport: ElementRef;
  IsSucess: boolean = false;
  IsError: boolean = false;
  is_progress: boolean = false;
  errorMsg: string;
  successMsg: string;
  saveBtnTxt: string = "Save";
  companyModel: CompanyMaster = new CompanyMaster();
  userModel: UserMaster = new UserMaster();
  selectedFile: ImageSnippet;
  maxSize: number = 1048576;

  constructor(private generalRepository: GeneralRepositoryService) { }

  ngOnInit(): void {
  }

  onSaveCompany() {
    this.is_progress = true;
    this.saveBtnTxt = "Saving...";
    this.userModel.roleId = roles.Admin;
    this.companyModel.userMaster.push(this.userModel);

    this.generalRepository.createCompany(this.companyModel).subscribe({
      next: data => (this.successMsg = "Company Created Successfully..", this.IsSucess = true, this.onSaveCompleted()),
      error: error => (this.errorMsg = error.error, this.IsError = true, this.onSaveCompleted())
    });
  }

  onSaveCompleted() {
    setTimeout(() => { this.closeAlert(); }, 5000);
    this.is_progress = false;
    this.saveBtnTxt = "Save";
  }

  closeAlert() {
    this.IsSucess = false;
    this.IsError = false;
  }

  processFile(imageInput: any) {
    const file: File = imageInput.files[0];
    const reader = new FileReader();

    reader.addEventListener('load', (event: any) => {

      this.selectedFile = new ImageSnippet(event.target.result, file);

      this.selectedFile.pending = true;
      //this.imageService.uploadImage(this.selectedFile.file).subscribe(
      //  (res) => {
      //    this.onSuccess();
      //  },
      //  (err) => {
      //    this.onError();
      //  })
    });

    reader.readAsDataURL(file);
  }

  onImageUpload(imageInput: any) {
    this.labelImport.nativeElement.innerText = imageInput.files[0].name;
    if (imageInput.files[0].size > this.maxSize) {
      this.errorMsg = "Logo is too large to upload. Maximum 1MB allowed"; this.IsError = true; this.onSaveCompleted();
      return;
    }
    const file: File = imageInput.files[0];
    const reader = new FileReader();

    //reader.addEventListener('load', (event: any) => {
    //  this.selectedFile = new ImageSnippet(event.target.result, file);
    //});
    //reader.readAsDataURL(file);


    //var r = new FileReader();
    reader.onload = () => {
      this.companyModel.logoBase64 = reader.result.toString();
    }
    reader.readAsDataURL(file);
  }
}
