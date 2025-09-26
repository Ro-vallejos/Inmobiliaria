using _net_integrador.Models;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioPropietario
    {
        List<Propietario> ObtenerPropietarios();
         List<Propietario> ObtenerPropietariosActivos();
        Propietario ObtenerPropietarioId(int id);
        Propietario ActualizarPropietario(Propietario propietario);
        void EliminarPropietario(int id);
        void AgregarPropietario(Propietario propietario);
        bool ExisteDni(string dni, int? idExcluido = null);
        bool ExisteEmail(string email, int? idExcluido = null);
    }
}