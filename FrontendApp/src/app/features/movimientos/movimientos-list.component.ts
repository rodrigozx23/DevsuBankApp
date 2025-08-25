import { Component, DestroyRef, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { MovimientosService } from '../../core/services/movimientos.service';
import { CuentasService } from '../../core/services/cuentas.service';
import { Cuenta } from '../../core/models/cuenta.model';
import { Movimiento } from '../../core/models/movimiento.model';

@Component({
  standalone: true,
  selector: 'app-movimientos-list',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './movimientos-list.component.html',
  styleUrls: ['./movimientos-list.component.css']
})
export class MovimientosListComponent {
  private destroyRef = inject(DestroyRef);
  private movSvc = inject(MovimientosService);
  private ctaSvc = inject(CuentasService);

  cuentas = signal<Cuenta[]>([]);
  cuentaId = signal<number | null>(null);
  desde = signal<string>('');  // yyyy-MM-dd
  hasta = signal<string>('');

  loading = signal(false);
  error = signal('');
  items = signal<Movimiento[]>([]);
  totCred = computed(() => this.items().filter(m => m.valor > 0).reduce((a, b) => a + b.valor, 0));
  totDeb = computed(() => this.items().filter(m => m.valor < 0).reduce((a, b) => a + Math.abs(b.valor), 0));
  saldoFin = computed(() => (this.items().at(-1)?.saldo) ?? 0);

  constructor() {
    this.ctaSvc.list('', 1, 1000)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: r => {
          this.cuentas.set(r.data);
          if (r.data.length && !this.cuentaId()) this.cuentaId.set(r.data[0].cuentaId);
        },
        error: () => this.error.set('No se pudieron cargar cuentas')
      });

    effect(() => {
      if (!this.cuentaId()) { this.items.set([]); return; }
      this.cargar();
    });
  }

  cargar() {
    if (!this.cuentaId()) return;
    this.loading.set(true);
    this.movSvc.list(this.cuentaId()!, this.desde() || undefined, this.hasta() || undefined)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: data => { this.items.set(data); this.loading.set(false); },
        error: e => { this.error.set(e?.error?.detail ?? 'Error'); this.items.set([]); this.loading.set(false); }
      });
  }
}