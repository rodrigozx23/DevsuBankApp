import { Component, DestroyRef, effect, inject, signal,computed  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ReportesService, EstadoReporte } from '../../core/services/reportes.service';
import { ClientesService } from '../../core/services/clientes.service';

type ReporteFila = {
  fecha: string | null;
  cliente: string;
  numero: string;
  tipoCuenta: string;
  saldoInicial: number;
  estado: boolean;
  movimiento: number;
  saldoDisponible: number;
};

@Component({
  standalone: true,
  selector: 'app-reporte-estado',
  imports: [CommonModule, FormsModule],
  templateUrl: './reporte-estado.component.html',
  styleUrls: ['./reporte-estado.component.css']
})

export class ReporteEstadoComponent {
  private destroyRef = inject(DestroyRef);
  private svc = inject(ReportesService);
  private cliSvc = inject(ClientesService);

  clienteId = signal<number | null>(null);
  clientes = signal<{ id: number; nombre: string }[]>([]);
  desde = signal<string>('');
  hasta = signal<string>('');

  loading = signal(false);
  error = signal('');
  data = signal<EstadoReporte | null>(null);

  filas = computed<ReporteFila[]>(() => {
    const d = this.data(); if (!d) return [];
    const out: ReporteFila[] = [];
    for (const c of d.cuentas) {
      if (!c.movimientos?.length) {
        out.push({
          fecha: null,
          cliente: d.clienteNombre,
          numero: c.numero,
          tipoCuenta: c.tipo,
          saldoInicial: c.saldoInicial,
          estado: (c as any).estado ?? true,
          movimiento: 0,
          saldoDisponible: c.saldoFinal
        });
        continue;
      }
      for (const m of c.movimientos) {
        out.push({
          fecha: m.fecha,
          cliente: d.clienteNombre,
          numero: c.numero,
          tipoCuenta: c.tipo,
          saldoInicial: c.saldoInicial,
          estado: (c as any).estado ?? true,
          movimiento: m.valor,
          saldoDisponible: m.saldo
        });
      }
    }
    return out;
  });

  constructor() {
    this.cliSvc.list('', 1, 100)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: r => this.clientes.set(r.data.map(x => ({ id: (x as any).clienteId ?? (x as any).personaId, nombre: x.nombre }))),
        error: () => this.error.set('No se pudieron cargar clientes')
      });
  }

  verJson() {
    if (!this.validar()) return;
    this.loading.set(true);
    this.svc.estadoCuenta(this.clienteId()!, this.desde()!, this.hasta()!)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: j => { this.data.set(j); this.loading.set(false); },
        error: e => { this.error.set(e?.error?.detail ?? 'Error'); this.data.set(null); this.loading.set(false); }
      });
  }

  descargarPdf() {
    if (!this.validar()) return;
    this.svc.estadoCuentaPDF(this.clienteId()!, this.desde()!, this.hasta()!)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: r => {
          const bytes = atob(r.data);
          const arr = new Uint8Array(bytes.length);
          for (let i = 0; i < bytes.length; i++) arr[i] = bytes.charCodeAt(i);
          const blob = new Blob([arr], { type: r.contentType });
          const url = URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url; a.download = r.fileName; a.click();
          URL.revokeObjectURL(url);
        },
        error: e => { this.error.set(e?.error?.detail ?? 'No se pudo descargar PDF'); }
      });
  }

  private validar() {
    this.error.set('');
    if (!this.clienteId()) { this.error.set('Seleccione un cliente'); return false; }
    if (!this.desde() || !this.hasta()) { this.error.set('Seleccione el rango de fechas'); return false; }
    return true;
  }

  totalCuentas() {
    const d = this.data(); if (!d) return 0;
    return d.cuentas.length;
  }
}