using _net_integrador.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace _net_integrador.Repositorios;

public class RepositorioTipoInmueble
{
    string ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";

    public List<TipoInmueble> ObtenerTiposInmueble()
    {
        List<TipoInmueble> tipos = new List<TipoInmueble>();
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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