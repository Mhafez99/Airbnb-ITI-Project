import { Injectable, isDevMode } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  constructor(private http: HttpClient) {}

  BASE_URL = isDevMode() ? 'https://localhost:7154/api/' : '/api/';
  headers = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: 'Bearer <token>',
  });
  private getHeaders() {
    const token = sessionStorage.getItem('token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
  }

  public get(endpoint: string, data?: any) {
    return this.httpRequest(endpoint, 'get', data);
  }

  public post(endpoint: string, data?: any) {
    return this.httpRequest(endpoint, 'post', data);
  }

  public put(endpoint: string, data: any) {
    return this.httpRequest(endpoint, 'put', data);
  }

  public delete(endpoint: string, data: any) {
    return this.httpRequest(endpoint, 'delete', data);
  }

  private httpRequest(endpoint: string, method: string, data: any = null) {
    const options = {
      body: data,
      headers: this.getHeaders(),
    };
    return this.http.request(method, `${this.BASE_URL}${endpoint}`, options);
  }
}
