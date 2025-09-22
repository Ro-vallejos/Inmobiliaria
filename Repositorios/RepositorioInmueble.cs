using _net_integrador.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace _net_integrador.Repositorios;

public class RepositorioInmueble
{
    string ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";

    public List<Inmueble> ObtenerInmuebles()
    {
        List<Inmueble> inmuebles = new List<Inmueble>();
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "SELECT id, id_propietario, direccion, uso, id_tipo, ambientes, eje_x, eje_y, precio, estado FROM inmueble";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    inmuebles.Add(new Inmueble
                    {
                        id = reader.GetInt32("id"),
                        id_propietario = reader.GetInt32("id_propietario"),
                        direccion = reader.GetString("direccion"),
                        uso = reader.GetString("uso"),
                        id_tipo = reader.GetInt32("id_tipo"),
                        ambientes = reader.GetInt32("ambientes"),
                        eje_x = reader.GetString("eje_x"),
                        eje_y = reader.GetString("eje_y"),
                        precio = reader.GetDecimal("precio"),
                        estado = reader.GetInt32("estado")
                    });
                }
                connection.Close();
            }
        }
        return inmuebles;
    }

    public Inmueble ObtenerInmuebleId(int id)
    {
        Inmueble inmueble = null;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var sql = "SELECT id, id_propietario, direccion, uso, id_tipo, ambientes, eje_x, eje_y, precio, estado FROM inmueble WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    inmueble = new Inmueble
                    {
                        id = reader.GetInt32("id"),
                        id_propietario = reader.GetInt32("id_propietario"),
                        direccion = reader.GetString("direccion"),
                        uso = reader.GetString("uso"),
                        id_tipo = reader.GetInt32("id_tipo"),
                        ambientes = reader.GetInt32("ambientes"),
                        eje_x = reader.GetString("eje_x"),
                        eje_y = reader.GetString("eje_y"),
                        precio = reader.GetDecimal("precio"),
                        estado = reader.GetInt32("estado")
                    };
                }
                connection.Close();
            }
        }
        return inmueble;
    }

    public void AgregarInmueble(Inmueble inmueble)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var sql = "INSERT INTO inmueble (id_propietario, direccion, uso, id_tipo, ambientes, eje_x, eje_y, precio, estado) VALUES (@id_propietario, @direccion, @uso, @id_tipo, @ambientes, @eje_x, @eje_y, @precio, @estado)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id_propietario", inmueble.id_propietario);
                command.Parameters.AddWithValue("@direccion", inmueble.direccion);
                command.Parameters.AddWithValue("@uso", inmueble.uso);
                command.Parameters.AddWithValue("@id_tipo", inmueble.id_tipo);
                command.Parameters.AddWithValue("@ambientes", inmueble.ambientes);
                command.Parameters.AddWithValue("@eje_x", inmueble.eje_x);
                command.Parameters.AddWithValue("@eje_y", inmueble.eje_y);
                command.Parameters.AddWithValue("@precio", inmueble.precio);
                command.Parameters.AddWithValue("@estado", inmueble.estado);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarInmueble(Inmueble inmueble)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var sql = "UPDATE inmueble SET id_propietario = @id_propietario, direccion = @direccion, uso = @uso, id_tipo = @id_tipo, ambientes = @ambientes, eje_x = @eje_x, eje_y = @eje_y, precio = @precio, estado = @estado WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", inmueble.id);
                command.Parameters.AddWithValue("@id_propietario", inmueble.id_propietario);
                command.Parameters.AddWithValue("@direccion", inmueble.direccion);
                command.Parameters.AddWithValue("@uso", inmueble.uso);
                command.Parameters.AddWithValue("@id_tipo", inmueble.id_tipo);
                command.Parameters.AddWithValue("@ambientes", inmueble.ambientes);
                command.Parameters.AddWithValue("@eje_x", inmueble.eje_x);
                command.Parameters.AddWithValue("@eje_y", inmueble.eje_y);
                command.Parameters.AddWithValue("@precio", inmueble.precio);
                command.Parameters.AddWithValue("@estado", inmueble.estado);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void SuspenderOferta(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "UPDATE inmueble SET estado = 0 WHERE id = @id";
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