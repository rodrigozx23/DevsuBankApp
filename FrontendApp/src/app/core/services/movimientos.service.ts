import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiHttpService } from './api-http.service';
import { Movimiento } from '../models/movimiento.model';


export interface MovimientoCreate {
  cuentaId: number;
  tipoMovimiento: 'Debito' | 'Credito';
  valor: number;
  fecha?: string;
}

@Injectable({ providedIn: 'root' })
export class MovimientosService {
  constructor(private api: ApiHttpService) {}

  list(cuentaId: number, desde?: string, hasta?: string) {
    const params: any = { cuentaId };
    if (desde) params.desde = desde;
    if (hasta) params.hasta = hasta;
    return this.api.get<Movimiento[]>('/movimientos', params);
  }

  create(dto: MovimientoCreate) {
    return this.api.post<Movimiento>('/movimientos', dto);
  }
}

