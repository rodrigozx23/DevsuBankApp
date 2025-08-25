export interface Cliente {
  clienteId: number;
  nombre: string;
  identificacion: string;
  estado: boolean;
  genero?: string | null;
  edad?: number | null;
  direccion?: string | null;
  telefono?: string | null;
  contrasena?: string | null;
}