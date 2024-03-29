import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { UserMasterComponent } from './user-master/user-master.component';
import { NewPwdComponent } from './new-pwd/new-pwd.component';
import { LoginComponent } from './login/login.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AuthGuard } from './services/auth-guard';
import { DashboardComponent } from './dashboard/dashboard.component';
import { NgbModule, NgbDateParserFormatter, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { JwtInterceptor } from './services/jwt-interceptor';
import { UserListComponent } from './user-list/user-list.component';
import { NgbdSortableHeader } from './services/sortable.directive';
import { DecimalPipe } from '@angular/common';
import { ForgetPasswordComponent } from './forget-password/forget-password.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { LeadCreateComponent } from './lead-create/lead-create.component';
import { LeadListComponent } from './lead-list/lead-list.component';
import { roles } from './services/generic-enums';
import { LeadFollowUpComponent } from './lead-follow-up/lead-follow-up.component';
import { DatePipe } from '@angular/common';
import { CustomDateParserFormatterService } from './services/custom-date-parser-formatter.service';
import { DashboardTelecallerComponent } from './dashboard-telecaller/dashboard-telecaller.component';
import { AdminSettingComponent } from './admin-setting/admin-setting.component';
import { DashboardManagerComponent } from './dashboard-manager/dashboard-manager.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { NgSelectModule } from '@ng-select/ng-select';
import { LeadListRmComponent } from './lead-list-rm/lead-list-rm.component';
import { TeleCallerSummaryComponent } from './tele-caller-summary/tele-caller-summary.component';
import { ReleManagerSummaryComponent } from './rele-manager-summary/rele-manager-summary.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { CompanyMasterComponent } from './company-master/company-master.component';
import { AdminCalenderComponent } from './admin-calender/admin-calender.component';
import { TargetEntryComponent } from './target-entry/target-entry.component';
import { TargetAchieveTeleCallerComponent } from './target-achieve-tele-caller/target-achieve-tele-caller.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    UserMasterComponent,
    NewPwdComponent,
    LoginComponent,
    DashboardComponent,
    UserListComponent,
    NgbdSortableHeader,
    ForgetPasswordComponent,
    ChangePasswordComponent,
    LeadCreateComponent,
    LeadListComponent,
    LeadFollowUpComponent,
    DashboardTelecallerComponent,
    AdminSettingComponent,
    DashboardManagerComponent,
    LeadListRmComponent,
    TeleCallerSummaryComponent,
    ReleManagerSummaryComponent,
    CompanyMasterComponent,
    AdminCalenderComponent,
    TargetEntryComponent,
    TargetAchieveTeleCallerComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    NgbModule,
    ReactiveFormsModule,
    HttpClientModule,
    FontAwesomeModule,
    FormsModule,
    InfiniteScrollModule,
    NgSelectModule,
    BrowserAnimationsModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory,
    }),
    RouterModule.forRoot([
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', component: LoginComponent },
      { path: 'new-pwd/:userId', component: NewPwdComponent },
      { path: 'forget-password', component: ForgetPasswordComponent },
      { path: 'chng-pwd', component: ChangePasswordComponent, canActivate: [AuthGuard] },

      { path: 'company-detail', component: CompanyMasterComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User'] } },
      { path: 'setting', component: AdminSettingComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },
      { path: 'user-detail', component: UserMasterComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },
      { path: 'user-list', component: UserListComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },
      { path: 'lead-create', component: LeadCreateComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Tele Caller'] } },
      { path: 'lead-list', component: LeadListComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Tele Caller', 'Admin'] } },
      { path: 'lead-list-rm', component: LeadListRmComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Relationship Manager', 'Admin'] } },
      { path: 'lead-followup/:callId', component: LeadFollowUpComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Relationship Manager'] } },
      { path: 'dash-tele', component: DashboardTelecallerComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Tele Caller'] } },
      { path: 'dash-manager', component: DashboardManagerComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Relationship Manager'] } },
      { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },
      { path: 'tele-caller-summary-rpt', component: TeleCallerSummaryComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },
      { path: 'rele-manager-summary-rpt', component: ReleManagerSummaryComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },
      { path: 'admin-calender', component: AdminCalenderComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },
      { path: 'target-entry', component: TargetEntryComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },
      { path: 'target-achieve-telecaller', component: TargetAchieveTeleCallerComponent, canActivate: [AuthGuard], data: { permittedRoles: ['Super User', 'Admin'] } },

      //{ path: 'counter', component: CounterComponent, canActivate: [AuthGuard] },
      //{ path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthGuard] },
    ]),
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatterService },
    DecimalPipe, DatePipe, NgbActiveModal
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
