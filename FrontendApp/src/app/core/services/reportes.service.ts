import { Injectable } from '@angular/core';
import { ApiHttpService } from './api-http.service';
import { Observable } from 'rxjs';

export interface EstadoCuentaMovimiento {
  fecha: string;
  tipo: 'Debito' | 'Credito';
  valor: number;
  saldo: number;
}

export interface EstadoCuenta {
  cuentaId: number;
  numero: string;
  tipo: string;
  saldoInicial: number;
  totalCreditos: number;
  totalDebitos: number;
  saldoFinal: number;
  movimientos: EstadoCuentaMovimiento[];
}

export interface EstadoReporte {
  clienteId: number;
  clienteNombre: string;
  desde: string;
  hasta: string;
  cuentas: EstadoCuenta[];
  totalCreditos: number;
  totalDebitos: number;
  saldoFinal: number;
}

@Injectable({ providedIn: 'root' })
export class ReportesService {
  constructor(private api: ApiHttpService) {}

  estadoCuenta(clienteId: number, desde: string, hasta: string) {
    return this.api.get<EstadoReporte>('/reportes/estado-cuenta', { clienteId, desde, hasta, format: 'json' });
  }

  estadoCuentaPDF(clienteId: number, desde: string, hasta: string) {
    return this.api.get<{ fileName: string; contentType: string; data: string }>(
      '/reportes/estado-cuenta',
      { clienteId, desde, hasta, format: 'pdf' }
    );
  }
}