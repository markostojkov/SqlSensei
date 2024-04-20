import { HttpHeaders, HttpClient, HttpParams, HttpRequest, HttpEventType } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, map, tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class BaseApiService {
  public httpHeaders: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
    Accept: 'application/json',
  });

  public constructor(private http: HttpClient) {}

  public get<T>(url: string, params: HttpParams = new HttpParams()): Observable<T> {
    return this.http.get<T>(`${environment.baseApiUrl}${url}`, {
      headers: this.httpHeaders,
      params,
    });
  }

  public getWithHeaderAppend<T>(url: string, name: string, value: string, params: HttpParams = new HttpParams()): Observable<T> {
    return this.http.get<T>(`${environment.baseApiUrl}${url}`, {
      headers: this.httpHeaders.append(name, value),
      params,
    });
  }

  public post<T>(url: string, data: Object = {}): Observable<T> {
    return this.http.post<T>(`${environment.baseApiUrl}${url}`, JSON.stringify(data), { headers: this.httpHeaders });
  }

  public put<T>(url: string, data: Object = {}): Observable<T> {
    return this.http.put<T>(`${environment.baseApiUrl}${url}`, JSON.stringify(data), { headers: this.httpHeaders });
  }

  public delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(`${environment.baseApiUrl}${url}`, {
      headers: this.httpHeaders,
    });
  }

  public patch<T>(url: string, data: Object = {}): Observable<T> {
    return this.http.patch<T>(`${environment.baseApiUrl}${url}`, JSON.stringify(data), { headers: this.httpHeaders });
  }
}
