import { Component, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CuentasService } from '../../core/services/cuentas.service';

@Component({
  standalone: true,
  selector: 'app-cuenta-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './cuenta-form.component.html',
  styleUrls: ['./cuenta-form.component.css']
})
export class CuentaFormComponent {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private svc = inject(CuentasService);
  private cdr = inject(ChangeDetectorRef);

  id = Number(this.route.snapshot.paramMap.get('id'));
  error = '';

  f = this.fb.nonNullable.group({
    clienteId: [0, Validators.required],
    numeroCuenta: ['', Validators.required],
    tipoCuenta: ['Ahorros', Validators.required],
    saldoInicial: [0, Validators.required],
    estado: [true]
  });

  ngOnInit() {
    if (this.id) {
      this.svc.get(this.id).subscribe({
        next: (c)=>{ this.f.patchValue(c); this.cdr.markForCheck(); },
        error: ()=>{ this.error='Cuenta no encontrada'; this.cdr.markForCheck(); }
      });
    }
  }

  guardar(){
    if (this.f.invalid){ this.f.markAllAsTouched(); return; }
    const dto = this.f.getRawValue();
    const ok = () => this.router.navigateByUrl('/cuentas');
    const fail = (e:any)=>{ this.error = e?.error?.detail ?? 'Error'; this.cdr.markForCheck(); };

    if (this.id) this.svc.update(this.id, dto).subscribe({next: ok, error: fail});
    else this.svc.create(dto).subscribe({next: ok, error: fail});
  }
  volver(){ this.router.navigateByUrl('/cuentas'); }
}