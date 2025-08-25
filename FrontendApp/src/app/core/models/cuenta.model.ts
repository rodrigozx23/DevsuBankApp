export interface Cuenta {
  cuentaId: number;
  numeroCuenta: string;
  tipoCuenta: string;
  saldoInicial: number;
  estado: boolean;
  clienteId: number;
  clienteNombre?: string;
}