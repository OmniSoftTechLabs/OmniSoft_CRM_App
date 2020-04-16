import { Injectable, PipeTransform } from '@angular/core';
import { SortColumn, SortDirection } from './sortable.directive';
import { BehaviorSubject, Subject, Observable, of } from 'rxjs';
import { DecimalPipe } from '@angular/common';
import { debounceTime, delay, switchMap, tap } from 'rxjs/operators';
import { UserMaster } from '../models/user-master';
import { LeadMaster } from '../models/lead-master';


interface SearchResult {
  dataList: any[];
  total: number;
}

interface State {
  page: number;
  pageSize: number;
  searchTerm: string;
  sortColumn: SortColumn;
  sortDirection: SortDirection;
}

const compare = (v1: string, v2: string) => v1 < v2 ? -1 : v1 > v2 ? 1 : 0;

function sort(dataList: any[], column: SortColumn, direction: string): any[] {
  if (direction === '' || column === '') {
    return dataList;
  } else {
    return [...dataList].sort((a, b) => {
      const res = compare(`${a[column].toString().toLocaleLowerCase()}`, `${b[column].toString().toLocaleLowerCase()}`);
      return direction === 'asc' ? res : -res;
    });
  }
}

function matches(objData: any, xType: UserMaster | LeadMaster, term: string, pipe: PipeTransform) {
  term = term.toLowerCase()
  if (xType instanceof UserMaster) {
    var objUser = objData as UserMaster;
    return objUser.firstName.toLowerCase().includes(term)
      || objUser.lastName.toLowerCase().includes(term)
      //|| objUser.email.toLowerCase().includes(term)
      || objUser.role.roleName.toLowerCase().includes(term)
    //|| objUser.status.toLowerCase().includes(term)
  }

  if (xType instanceof LeadMaster) {
    var objlead = objData as LeadMaster;
    return objlead.firstName.toLowerCase().includes(term)
      || objlead.lastName.toLowerCase().includes(term)
      || objlead.mobileNumber.toLowerCase().includes(term)
  }

  //|| pipe.transform(objData.lastName).includes(term)
  //|| pipe.transform(objData.email).includes(term);
}


@Injectable({
  providedIn: 'root'
})
export class DataTableService {

  private _loading$ = new BehaviorSubject<boolean>(true);
  private _search$ = new Subject<void>();
  private _dataList$ = new BehaviorSubject<any[]>([]);
  private _total$ = new BehaviorSubject<number>(0);

  public TABLE: any[];
  public xType: UserMaster | LeadMaster;
  private _state: State = {
    page: 1,
    pageSize: 10,
    searchTerm: '',
    sortColumn: '',
    sortDirection: ''
  };


  constructor(private pipe: DecimalPipe) {

    //this._dataList$ = new BehaviorSubject<any[]>([]);

    this._search$.pipe(
      tap(() => this._loading$.next(true)),
      delay(500),
      //debounceTime(500),
      switchMap(() => this._search()),
      delay(200),
      tap(() => this._loading$.next(false))
    ).subscribe(result => {

      this._dataList$.next(result.dataList);
      this._total$.next(result.total);
    });

    this._search$.next();
  }

  get dataList$() { return this._dataList$.asObservable(); }
  get total$() { return this._total$.asObservable(); }
  get loading$() { return this._loading$.asObservable(); }
  get page() { return this._state.page; }
  get pageSize() { return this._state.pageSize; }
  get searchTerm() { return this._state.searchTerm; }

  set page(page: number) { this._set({ page }); }
  set pageSize(pageSize: number) { this._set({ pageSize }); }
  set searchTerm(searchTerm: string) { this._set({ searchTerm }); }
  set sortColumn(sortColumn: SortColumn) { this._set({ sortColumn }); }
  set sortDirection(sortDirection: SortDirection) { this._set({ sortDirection }); }

  private _set(patch: Partial<State>) {
    Object.assign(this._state, patch);
    this._search$.next();
  }

  private _search(): Observable<SearchResult> {
    const { sortColumn, sortDirection, pageSize, page, searchTerm } = this._state;


    // 1. sort
    let dataList = sort(this.TABLE, sortColumn, sortDirection);

    // 2. filter
    dataList = dataList.filter(objData => matches(objData, this.xType, searchTerm, this.pipe));
    const total = dataList.length;

    // 3. paginate
    dataList = dataList.slice((page - 1) * pageSize, (page - 1) * pageSize + pageSize);
    return of({ dataList, total });
  }

}
