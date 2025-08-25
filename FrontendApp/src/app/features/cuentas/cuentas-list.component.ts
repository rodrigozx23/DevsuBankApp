import { Component, signal, effect, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CuentasService, CuentasListResponse } from '../../core/services/cuentas.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Cuenta } from '../../core/models/cuenta.model';

@Component({
  standalone: true,
  selector: 'app-cuentas-list',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './cuentas-list.component.html',
  styleUrls: ['./cuentas-list.component.css']
})
export class CuentasListComponent {
  private destroyRef = inject(DestroyRef);
  constructor(private svc: CuentasService, private router: Router) { effect(() => this.cargar()); }
  q = signal(''); page = signal(1); pageSize = signal(10);
  total = signal(0); items = signal<Cuenta[]>([]); loading = signal(false); error = signal('');

  cargar() {
    this.loading.set(true);
    this.svc.list(this.q(), this.page(), this.pageSize())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (r:CuentasListResponse)=>{this.items.set(r.data); this.total.set(r.total); this.loading.set(false);},
        error: (e)=>{this.error.set(e?.error?.detail ?? 'Error'); this.items.set([]); this.total.set(0); this.loading.set(false);}
      });
  }
  buscar(){ this.page.set(1); }
  editar(id:number){ this.router.navigate(['/cuentas', id]); }
  eliminar(id:number){ if(confirm('¿Eliminar cuenta?')) this.svc.delete(id).subscribe({next:()=>this.cargar()}); }
}