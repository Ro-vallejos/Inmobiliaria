using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioContrato
    {
        List<Contrato> ObtenerContratos();
        Contrato? ObtenerContratoId(int id);
        void AgregarContrato(Contrato contrato);
        void ActualizarContrato(Contrato contrato);
        List<Contrato> ObtenerContratoPorInmueble(int idInmueble, int idContrato);
    }
}
