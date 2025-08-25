import { Component, signal, computed, effect, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ClientesService, ClientesListResponse } from '../../core/services/clientes.service';
import { Cliente } from '../../core/models/cliente.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  standalone: true,
  selector: 'app-clientes-list',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './clientes-list.component.html',
  styleUrls: ['./clientes-list.component.css']
})
export class ClientesListComponent {

  private destroyRef = inject(DestroyRef);
  constructor(private svc: ClientesService, private router: Router) {
    effect(() => this.cargar());
  }

  q = signal('');
  page = signal(1);
  pageSize = signal(10);
  total = signal(0);
  items = signal<Cliente[]>([]);
  loading = signal(false);
  error = signal('');

  cargar() {
    this.loading.set(true);
    this.svc.list(this.q(), this.page(), this.pageSize()).subscribe({
      next: (r: ClientesListResponse) => { 
        this.items.set(r.data); this.total.set(r.total); this.loading.set(false); 
        if (this.q().trim() && r.total === 1 && r.data.length === 1) {
            this.router.navigate(['/clientes', r.data[0].clienteId]);
        }
      },
      error: (e) => { this.error.set(e?.error?.detail ?? 'Error'); this.items.set([]); this.total.set(0); this.loading.set(false); }
    });
  }

  buscar() { this.page.set(1); }

  eliminar(id: number) {
    if (!confirm('¿Eliminar cliente?')) return;
    this.svc.delete(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => this.cargar(),
        error: () => alert('No se pudo eliminar')
    });
  }

  editar(id: number) { this.router.navigate(['/clientes', id]); }

  pages = computed(() => Math.max(1, Math.ceil(this.total() / this.pageSize())));
}