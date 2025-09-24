using _net_integrador.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {

        public RepositorioInmueble(IConfiguration configuration) : base(configuration) { }

        public List<Inmueble> ObtenerInmuebles()
        {
            List<Inmueble> inmuebles = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "SELECT i.id, i.direccion, i.uso, i.id_tipo, i.precio, i.estado, p.nombre AS nombrePropietario, p.apellido AS apellidoPropietario, t.tipo AS tipoInmueble FROM inmueble i JOIN propietario p ON i.id_propietario = p.id AND p.estado = 1 JOIN tipo_inmueble t ON i.id_tipo = t.id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            id = reader.GetInt32("id"),
                            uso = Enum.Parse<UsoInmueble>(reader.GetString("uso")),
                            id_tipo = reader.GetInt32("id_tipo"),
                            precio = reader.GetDecimal("precio"),
                            estado = reader.GetInt32("estado"),
                            direccion = reader.GetString("direccion"),
                            propietario = new Propietario
                            {
                                nombre = reader.GetString("nombrePropietario"),
                                apellido = reader.GetString("apellidoPropietario")
                            },
                            tipoInmueble = new TipoInmueble
                            {
                                id = reader.GetInt32("id_tipo"),
                                tipo = reader.GetString("tipoInmueble")
                            }
                        });
                    }
                    connection.Close();
                }
            }
            return inmuebles;
        }

        public Inmueble? ObtenerInmuebleId(int id)
        {
            Inmueble? inmueble = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
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
                            uso = Enum.Parse<UsoInmueble>(reader.GetString("uso")),
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

        public void AgregarInmueble(Inmueble inmuebleNuevo)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    var sql = "INSERT INTO inmueble (id_propietario, direccion, uso, id_tipo, ambientes, eje_x, eje_y, precio, estado) VALUES (@id_propietario, @direccion, @uso, @id_tipo, @ambientes, @eje_x, @eje_y, @precio, @estado)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@id_propietario", inmuebleNuevo.id_propietario);
                        command.Parameters.AddWithValue("@direccion", inmuebleNuevo.direccion);
                        command.Parameters.AddWithValue("@uso", inmuebleNuevo.uso.ToString());
                        command.Parameters.AddWithValue("@id_tipo", inmuebleNuevo.id_tipo);
                        command.Parameters.AddWithValue("@ambientes", inmuebleNuevo.ambientes);
                        command.Parameters.AddWithValue("@eje_x", inmuebleNuevo.eje_x);
                        command.Parameters.AddWithValue("@eje_y", inmuebleNuevo.eje_y);
                        command.Parameters.AddWithValue("@precio", inmuebleNuevo.precio);
                        command.Parameters.AddWithValue("@estado", inmuebleNuevo.estado);
                        command.ExecuteNonQuery();
                        connection.Close();
                        Console.WriteLine("Inmueble agregado correctamente.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error SQL: {ex.Message}");
            }
        }

        public void ActualizarInmueble(Inmueble inmuebleEditado)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE inmueble SET id_propietario = @id_propietario, direccion = @direccion, uso = @uso, id_tipo = @id_tipo, ambientes = @ambientes, eje_x = @eje_x, eje_y = @eje_y, precio = @precio, estado = @estado WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", inmuebleEditado.id);
                    command.Parameters.AddWithValue("@id_propietario", inmuebleEditado.id_propietario);
                    command.Parameters.AddWithValue("@direccion", inmuebleEditado.direccion);
                    command.Parameters.AddWithValue("@uso", inmuebleEditado.uso.ToString());
                    command.Parameters.AddWithValue("@id_tipo", inmuebleEditado.id_tipo);
                    command.Parameters.AddWithValue("@ambientes", inmuebleEditado.ambientes);
                    command.Parameters.AddWithValue("@eje_x", inmuebleEditado.eje_x);
                    command.Parameters.AddWithValue("@eje_y", inmuebleEditado.eje_y);
                    command.Parameters.AddWithValue("@precio", inmuebleEditado.precio);
                    command.Parameters.AddWithValue("@estado", inmuebleEditado.estado);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void SuspenderOferta(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
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
        public void ActivarOferta(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "UPDATE inmueble SET estado = 1 WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public List<Inmueble> ObtenerInmueblesDisponibles()
        {
            List<Inmueble> inmuebles = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = @"
                    SELECT 
                        i.id, i.direccion, i.uso, i.id_tipo, i.precio, i.estado, 
                        p.nombre AS nombrePropietario, p.apellido AS apellidoPropietario, 
                        t.tipo AS tipoInmueble
                    FROM inmueble i
                    JOIN propietario p ON i.id_propietario = p.id AND p.estado = 1
                    JOIN tipo_inmueble t ON i.id_tipo = t.id
                    WHERE i.estado = 1 AND i.id NOT IN (SELECT id_inmueble FROM contrato WHERE estado = 1)"; 

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            id = reader.GetInt32("id"),
                            uso = Enum.Parse<UsoInmueble>(reader.GetString("uso")),
                            id_tipo = reader.GetInt32("id_tipo"),
                            precio = reader.GetDecimal("precio"),
                            estado = reader.GetInt32("estado"),
                            direccion = reader.GetString("direccion"),
                            propietario = new Propietario
                            {
                                nombre = reader.GetString("nombrePropietario"),
                                apellido = reader.GetString("apellidoPropietario")
                            },
                            tipoInmueble = new TipoInmueble
                            {
                                id = reader.GetInt32("id_tipo"),
                                tipo = reader.GetString("tipoInmueble")
                            }
                        });
                    }
                    connection.Close();
                }
            }
            return inmuebles;
        }
    }
}
