import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Filter } from '../models/filter';
import { Stats } from '../models/stats';

@Injectable({
  providedIn: 'root'
})
export class FilterService {

  private srvUrl = 'https://spam-scripts.mczernecki.pl';

  constructor(private http: HttpClient) { }

  private headers = new HttpHeaders()
    .set('Content-Type', 'application/json; charset=utf-8')
  //.set('LA-Device', 'q3p498f8ntbnpvw98ytvpo');

  getSenders(): Observable<Filter[]> {
    return this.http.post<Filter[]>(`${this.srvUrl}/filter-get.php`, { type: 'senders' }, { headers: this.headers });
  }

  getSubjects(): Observable<Filter[]> {
    return this.http.post<Filter[]>(`${this.srvUrl}/filter-get.php`, { type: 'subjects' }, { headers: this.headers });
  }

  getDomains(): Observable<Filter[]> {
    return this.http.post<Filter[]>(`${this.srvUrl}/filter-get.php`, { type: 'domains' }, { headers: this.headers });
  }

  add(rule: Filter): Observable<HttpResponse<any>> {
    return this.http.post<any>(`${this.srvUrl}/filter-add.php`, { type: rule.type, value: rule.value }, { headers: this.headers, observe: 'response' });
  }

  delete(id: number): Observable<HttpResponse<any>> {
    return this.http.post<any>(`${this.srvUrl}/filter-delete.php`, { id: id }, { headers: this.headers, observe: 'response' });
  }

  getStats(): Observable<Stats> {
    return this.http.post<any>(`${this.srvUrl}/stats.php`, {}, { headers: this.headers })
    .pipe(
      map(response => response.details)
    );
  }

}
