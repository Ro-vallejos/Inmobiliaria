using _net_integrador.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace _net_integrador.Repositorios;

public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
{
    public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration) { }

    public List<TipoInmueble> ObtenerTiposInmueble()
    {
        List<TipoInmueble> tipos = new List<TipoInmueble>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT id, tipo FROM tipo_inmueble";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tipos.Add(new TipoInmueble
                    {
                        id = reader.GetInt32("id"),
                        tipo = reader.GetString("tipo")
                    });
                }
                connection.Close();
            }
        }
        return tipos;
    }

    public TipoInmueble? ObtenerTipoInmuebleId(int id)
    {
        TipoInmueble? tipoInmueble = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT id, tipo FROM tipo_inmueble WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    tipoInmueble = new TipoInmueble
                    {
                        id = reader.GetInt32("id"),
                        tipo = reader.GetString("tipo")
                    };
                }
                connection.Close();
            }
        }
        return tipoInmueble;
    }

    public void AgregarTipoInmueble(TipoInmueble tipo)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "INSERT INTO tipo_inmueble (tipo) VALUES (@tipo)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@tipo", tipo.tipo);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarTipoInmueble(TipoInmueble tipo)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "UPDATE tipo_inmueble SET tipo = @tipo WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", tipo.id);
                command.Parameters.AddWithValue("@tipo", tipo.tipo);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void EliminarTipoInmueble(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "DELETE FROM tipo_inmueble WHERE id = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
