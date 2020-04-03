import { Injectable, PipeTransform } from '@angular/core';
import { SortColumn, SortDirection } from './sortable.directive';
import { BehaviorSubject, Subject, Observable, of } from 'rxjs';
import { DecimalPipe } from '@angular/common';
import { debounceTime, delay, switchMap, tap } from 'rxjs/operators';


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
      const res = compare(`${a[column]}`, `${b[column]}`);
      return direction === 'asc' ? res : -res;
    });
  }
}

function matches(objData: any, term: string, pipe: PipeTransform) {

  return objData.firstName.toLowerCase().includes(term.toLowerCase())
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

  private _state: State = {
    page: 1,
    pageSize: 10,
    searchTerm: '',
    sortColumn: '',
    sortDirection: ''
  };


  constructor(private pipe: DecimalPipe) {


    this._search$.pipe(
      tap(() => this._loading$.next(true)),
      debounceTime(500),
      switchMap(() => this._search()),
      delay(500),
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
    dataList = dataList.filter(objData => matches(objData, searchTerm, this.pipe));
    const total = dataList.length;

    // 3. paginate
    dataList = dataList.slice((page - 1) * pageSize, (page - 1) * pageSize + pageSize);
    return of({ dataList, total });
  }

}
