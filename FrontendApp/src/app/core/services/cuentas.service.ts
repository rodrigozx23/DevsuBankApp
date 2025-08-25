import { Injectable } from '@angular/core';
import { ApiHttpService } from './api-http.service';
import { Observable } from 'rxjs';
import { Cuenta } from '../models/cuenta.model';

export interface CuentasListResponse { total: number; data: Cuenta[]; }

@Injectable({ providedIn: 'root' })
export class CuentasService {
  constructor(private api: ApiHttpService) {}
  list(q?: string, page=1, pageSize=10) { return this.api.get<CuentasListResponse>('/cuentas', { q, page, pageSize }); }
  listByCliente(clienteId: number) { return this.api.get<Cuenta[]>('/cuentas/by-cliente/'+clienteId); }
  get(id: number) { return this.api.get<Cuenta>('/cuentas/'+id); }
  create(dto: Partial<Cuenta>) { return this.api.post<Cuenta>('/cuentas', dto); }
  update(id: number, dto: Partial<Cuenta>) { return this.api.put<void>('/cuentas/'+id, dto); }
  delete(id: number) { return this.api.delete<void>('/cuentas/'+id); }
}

