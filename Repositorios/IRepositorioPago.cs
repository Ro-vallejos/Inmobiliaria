using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioPago
    {
        List<Pago> ObtenerPagosPorContrato(int contratoId);
        Pago? ObtenerPagoId(int id);
        void AgregarPago(Pago pago);
        void AnularPago(int id);
    }
}
