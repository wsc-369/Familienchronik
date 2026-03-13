import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Address } from '../api/api-interfaces';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AddressService {
  private readonly baseUrl = `${environment.apiUrl}/Address`;

  constructor(private readonly http: HttpClient) {}

  getAddresses(): Observable<Address[]> {
    return this.http.get<Address[]>(this.baseUrl);
  }

  getAddressById(id: string): Observable<Address> {
    return this.http.get<Address>(`${this.baseUrl}/${encodeURIComponent(id)}`);
  }

  createAddress(address: Address): Observable<Address> {
    return this.http.post<Address>(this.baseUrl, address);
  }

  updateAddress(id: string, address: Address): Observable<Address> {
    return this.http.put<Address>(`${this.baseUrl}/${encodeURIComponent(id)}`, address);
  }

  deleteAddress(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${encodeURIComponent(id)}`);
  }
}
