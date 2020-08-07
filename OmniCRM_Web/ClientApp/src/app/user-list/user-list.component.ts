import { Component, OnInit, QueryList, ViewChildren, PipeTransform } from '@angular/core';
import { GeneralRepositoryService } from '../services/general-repository.service';
import { UserMaster } from '../models/user-master';
import { Observable, timer } from 'rxjs';
import { NgbdSortableHeader, SortEvent } from '../services/sortable.directive';
import { DataTableService } from '../services/data-table.service';
import { FormControl } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { startWith, map } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserMasterComponent } from '../user-master/user-master.component';
import { Router } from '@angular/router';

function search(userlist: UserMaster[], text: string, pipe: PipeTransform): UserMaster[] {

  //let userArray: UserMaster[];
  //userlist.subscribe(data => userArray = data);

  return userlist.filter(user => {
    const term = text.toLowerCase();
    return user.firstName.toLowerCase().includes(term)
      || user.lastName.toLowerCase().includes(term)
      || user.email.toLowerCase().includes(term)
      || user.role.roleName.toLowerCase().includes(term)
    //|| pipe.transform(user.lastName).includes(term) // use for number filter
    //|| pipe.transform(user.email.toString()).includes(term);
  });
}

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  userList: Observable<UserMaster[]>;
  total$: Observable<number>;
  filter = new FormControl('');
  modalTitle: string = "Add User";
  @ViewChildren(NgbdSortableHeader) headers: QueryList<NgbdSortableHeader>;

  //userList: UserMaster[] = [];

  constructor(private generalRepository: GeneralRepositoryService, public service: DataTableService, private pipe: DecimalPipe, private modalService: NgbModal, private router: Router) {
  }

  ngOnInit(): void {
    this.fillUserList()
    this.service.searchTerm = '';
  }

  fillUserList() {
    this.service.TABLE = [];
    this.generalRepository.loadUserList().subscribe(
      (users) => {
        //setTimeout(() => { this.service.TABLE = users }, 100);
        //timer(100).subscribe(x => { this.service.TABLE = users })
        this.service.xType = new UserMaster();
        this.service.TABLE = users;
        this.userList = this.service.dataList$;
        //this.filteredUserList = this.filter.valueChanges.pipe(startWith(''), map(text => search(users, text, this.pipe)));
        this.total$ = this.service.total$;
      },
      error => console.error(error)
    );
  }



  onSort({ column, direction }: SortEvent) {
    // resetting other headers
    this.headers.forEach(header => {
      if (header.sortable !== column) {
        header.direction = '';
      }
    });

    this.service.sortColumn = column;
    this.service.sortDirection = direction;
  }

  edituser(userId: string) {
    //this.modalService.open(UserMasterComponent, { size: 'lg' });
    //this.modalTitle = "Edit User";
    localStorage.removeItem("userId");
    localStorage.setItem("userId", userId.toString());
    this.router.navigate(['/user-detail']);
  }

}
