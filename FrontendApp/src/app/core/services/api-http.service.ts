import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../enviromments/environment';

@Injectable({ providedIn: 'root' })
export class ApiHttpService {
  private base = environment.api;
  constructor(private http: HttpClient) {}
  get<T>(u: string, params?: Record<string, any>) {
    let p = new HttpParams();
    if (params) for (const [k, v] of Object.entries(params)) if (v != null) p = p.set(k, String(v));
    return this.http.get<T>(`${this.base}${u}`, { params: p });
  }
  post<T>(u: string, b: unknown) { return this.http.post<T>(`${this.base}${u}`, b); }
  put<T>(u: string, b: unknown)  { return this.http.put<T>(`${this.base}${u}`, b); }
  delete<T>(u: string)           { return this.http.delete<T>(`${this.base}${u}`); }
}