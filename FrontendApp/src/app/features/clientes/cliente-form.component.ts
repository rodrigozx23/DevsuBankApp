import { Component, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientesService } from '../../core/services/clientes.service';

@Component({
  standalone: true,
  selector: 'app-cliente-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './cliente-form.component.html',
  styleUrls: ['./cliente-form.component.css']
})


export class ClienteFormComponent {
    private fb = inject(FormBuilder);
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private svc = inject(ClientesService);
    private cdr = inject(ChangeDetectorRef);

    id = Number(this.route.snapshot.paramMap.get('id'));
    error = '';

    f = this.fb.nonNullable.group({
        nombre: ['', [Validators.required, Validators.minLength(2)]],
        identificacion: ['', Validators.required],
        genero: 'M',
        edad: 18,
        direccion: '',
        telefono: '',
        contrasena: ['', [Validators.required, Validators.minLength(4)]],
        estado: true
    });

    ngOnInit() {
        if (this.id) {
        this.svc.get(this.id).subscribe({
            next: (c) => {
            this.f.patchValue({
                nombre: c.nombre,
                identificacion: c.identificacion,
                genero: c.genero ?? 'M',
                edad: c.edad ?? 18,
                direccion: c.direccion ?? '',
                telefono: c.telefono ?? '',
                contrasena: c.contrasena ?? '',
                estado: c.estado
            });
            this.cdr.markForCheck(); // zoneless
            },
            error: () => { this.error = 'Cliente no encontrado'; this.cdr.markForCheck(); }
        });
        }
    }

    guardar() {
        if (this.f.invalid) { this.f.markAllAsTouched(); return; }
        const dto = this.f.getRawValue();

        if (this.id) {
            this.svc.update(this.id, dto).subscribe({
            next: () => this.router.navigateByUrl('/clientes'),
            error: (e) => this.error = e?.error?.detail ?? 'Error al guardar'
            });
        } else {
            this.svc.create(dto).subscribe({
            next: () => this.router.navigateByUrl('/clientes'),
            error: (e) => this.error = e?.error?.detail ?? 'Error al guardar'
            });
        }
    }
    volver() {
        this.router.navigateByUrl('/clientes');
    }
}
