using _net_integrador.Models;

namespace _net_integrador.Repositorios;

public interface IRepositorioAuditoria
{
    void InsertarRegistroAuditoria(string tipo, int idRegistro, AccionAuditoria accion, string usuario);
}