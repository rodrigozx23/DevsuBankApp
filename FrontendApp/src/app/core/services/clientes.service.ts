import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiHttpService } from './api-http.service';
import { Cliente } from '../models/cliente.model';

export interface ClientesListResponse { total: number; data: Cliente[]; }

@Injectable({ providedIn: 'root' })
export class ClientesService {
  constructor(private api: ApiHttpService) {}
  list(q?: string, page=1, pageSize=10): Observable<ClientesListResponse> {
    return this.api.get('/clientes', { q, page, pageSize });
  }
  get(id: number) { return this.api.get<Cliente>(`/clientes/${id}`); }
  create(dto: Partial<Cliente>) { return this.api.post<Cliente>('/clientes', dto); }
  update(id: number, dto: Partial<Cliente>) { return this.api.put<void>(`/clientes/${id}`, dto); }
  delete(id: number) { return this.api.delete<void>(`/clientes/${id}`); }
}