export interface Movimiento {
  movimientoId: number;
  fecha: string;
  tipoMovimiento: 'Debito' | 'Credito';
  valor: number;
  saldo: number;
  cuentaId: number;
}
