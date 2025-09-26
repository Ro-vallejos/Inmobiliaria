using _net_integrador.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace _net_integrador.Repositorios;

public class RepositorioAuditoria : RepositorioBase, IRepositorioAuditoria
{
    public RepositorioAuditoria(IConfiguration configuration) : base(configuration) { }

    public void InsertarRegistroAuditoria(string tipo, int idRegistro, AccionAuditoria accion, string usuario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "INSERT INTO auditoria (tipo, id_registro_afectado, accion, usuario, fecha_hora) VALUES (@tipo, @id_registro, @accion, @usuario, @fecha_hora)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@tipo", tipo);
                command.Parameters.AddWithValue("@id_registro", idRegistro);
                command.Parameters.AddWithValue("@accion", accion.ToString()); 
                command.Parameters.AddWithValue("@usuario", usuario);
                command.Parameters.AddWithValue("@fecha_hora", DateTime.Now);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}