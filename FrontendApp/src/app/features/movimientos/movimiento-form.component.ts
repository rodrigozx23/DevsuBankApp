import { Component, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { MovimientosService } from '../../core/services/movimientos.service';
import { CuentasService } from '../../core/services/cuentas.service';
import { Movimiento } from '../../core/models/movimiento.model';
import { Cuenta } from '../../core/models/cuenta.model';

@Component({
  standalone: true,
  selector: 'app-movimiento-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './movimiento-form.component.html',
  styleUrls: ['./movimiento-form.component.css']
})
export class MovimientoFormComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private svc = inject(MovimientosService);
  private ctaSvc = inject(CuentasService);
  private cdr = inject(ChangeDetectorRef);

  cuentas: Cuenta[] = [];

  error = '';

  f = this.fb.nonNullable.group({
    cuentaId: [0, Validators.required],
    tipoMovimiento: ['Credito' as 'Credito' | 'Debito', Validators.required],
    valor: [0, [Validators.required, Validators.min(0.01)]],
    fecha: ['']
  });

  constructor() {
    this.ctaSvc.list('', 1, 1000)
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: r => { this.cuentas = r.data; if (this.cuentas.length && !this.f.value.cuentaId) this.f.patchValue({ cuentaId: this.cuentas[0].cuentaId }); this.cdr.markForCheck(); },
        error: () => { this.error = 'No se pudieron cargar cuentas'; this.cdr.markForCheck(); }
      });
  }

  guardar() {
    if (this.f.invalid) { this.f.markAllAsTouched(); return; }
    const dto = this.f.getRawValue();
    this.svc.create(dto).subscribe({
      next: () => this.router.navigateByUrl('/movimientos'),
      error: (e) => { this.error = e?.error?.detail ?? 'Error al registrar movimiento'; this.cdr.markForCheck(); }
    });
  }

  volver() { this.router.navigateByUrl('/movimientos'); }
}