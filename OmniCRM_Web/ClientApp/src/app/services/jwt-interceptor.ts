import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { Router } from '@angular/router';
import { tap } from "rxjs/operators";


@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService, private router: Router) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // add authorization header with jwt token if available
    let currentUser = this.authenticationService.currentUserValue;
    if (currentUser && currentUser.token) {
      request = request.clone({
        headers: request.headers.set('Authorization', 'Bearer ' + currentUser.token)
        //setHeaders: {
        //  Authorization: 'Bearer ${currentUser.token}'
        //}
      });
      return next.handle(request).pipe(
        tap(
          succ => { },
          err => {
            if (err.status == 401) {
              localStorage.removeItem('currentUser');
              this.router.navigateByUrl('/login');
            }
            else if (err.status == 403)
              this.router.navigateByUrl('/login');
          }
        )
      )
    }

    return next.handle(request.clone());
  }
}
