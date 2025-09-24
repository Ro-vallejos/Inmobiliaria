using _net_integrador.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace _net_integrador.Repositorios;

public class RepositorioContrato : RepositorioBase, IRepositorioContrato
{
    public RepositorioContrato(IConfiguration configuration) : base(configuration) { }

    public List<Contrato> ObtenerContratos()
    {
        List<Contrato> contratos = new List<Contrato>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"
                SELECT c.id, c.id_inquilino, c.id_inmueble, c.fecha_inicio, c.fecha_fin, c.incremento, c.estado, 
                       i.nombre AS nombreInquilino, i.apellido AS apellidoInquilino, i.dni AS dniInquilino, i.email AS emailInquilino, i.telefono AS telefonoInquilino, i.estado AS estadoInquilino, 
                       inm.direccion
                FROM contrato c
                JOIN inquilino i ON c.id_inquilino = i.id
                JOIN inmueble inm ON c.id_inmueble = inm.id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    contratos.Add(new Contrato
                    {
                        id = reader.GetInt32("id"),
                        id_inquilino = reader.GetInt32("id_inquilino"),
                        id_inmueble = reader.GetInt32("id_inmueble"),
                        fecha_inicio = reader.GetDateTime("fecha_inicio"),
                        fecha_fin = reader.GetDateTime("fecha_fin"),
                        incremento = reader.GetDecimal("incremento"),
                        estado = reader.GetInt32("estado"),
                        Inquilino = new Inquilino
                        {
                            id = reader.GetInt32("id_inquilino"),
                            nombre = reader.GetString("nombreInquilino"),
                            apellido = reader.GetString("apellidoInquilino"),
                            dni = reader.GetString("dniInquilino"),
                            email = reader.GetString("emailInquilino"),
                            telefono = reader.GetString("telefonoInquilino"),
                            estado = reader.GetInt32("estadoInquilino")
                        },
                        Inmueble = new Inmueble
                        {
                            id = reader.GetInt32("id_inmueble"),
                            direccion = reader.GetString("direccion")
                        }
                    });
                }
                connection.Close();
            }
        }
        return contratos;
    }
    
    public Contrato? ObtenerContratoId(int id)
    {
        Contrato? contrato = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = @"
                SELECT c.id, c.id_inquilino, c.id_inmueble, c.fecha_inicio, c.fecha_fin, c.incremento, c.estado,
                       i.nombre AS nombreInquilino, i.apellido AS apellidoInquilino, inm.direccion
                FROM contrato c
                JOIN inquilino i ON c.id_inquilino = i.id
                JOIN inmueble inm ON c.id_inmueble = inm.id
                WHERE c.id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    contrato = new Contrato
                    {
                        id = reader.GetInt32("id"),
                        id_inquilino = reader.GetInt32("id_inquilino"),
                        id_inmueble = reader.GetInt32("id_inmueble"),
                        fecha_inicio = reader.GetDateTime("fecha_inicio"),
                        fecha_fin = reader.GetDateTime("fecha_fin"),
                        incremento = reader.GetDecimal("incremento"),
                        estado = reader.GetInt32("estado"),
                        Inquilino = new Inquilino
                        {
                            id = reader.GetInt32("id_inquilino"),
                            nombre = reader.GetString("nombreInquilino"),
                            apellido = reader.GetString("apellidoInquilino")
                        },
                        Inmueble = new Inmueble
                        {
                            id = reader.GetInt32("id_inmueble"),
                            direccion = reader.GetString("direccion")
                        }
                    };
                }
                connection.Close();
            }
        }
        return contrato;
    }

    public void AgregarContrato(Contrato contrato)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "INSERT INTO contrato (id_inquilino, id_inmueble, fecha_inicio, fecha_fin, incremento, estado) VALUES (@id_inquilino, @id_inmueble, @fecha_inicio, @fecha_fin, @incremento, @estado)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id_inquilino", contrato.id_inquilino);
                command.Parameters.AddWithValue("@id_inmueble", contrato.id_inmueble);
                command.Parameters.AddWithValue("@fecha_inicio", contrato.fecha_inicio);
                command.Parameters.AddWithValue("@fecha_fin", contrato.fecha_fin);
                command.Parameters.AddWithValue("@incremento", contrato.incremento);
                command.Parameters.AddWithValue("@estado", contrato.estado);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarContrato(Contrato contrato)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "UPDATE contrato SET id_inquilino = @id_inquilino, id_inmueble = @id_inmueble, fecha_inicio = @fecha_inicio, fecha_fin = @fecha_fin, incremento = @incremento, estado = @estado WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", contrato.id);
                command.Parameters.AddWithValue("@id_inquilino", contrato.id_inquilino);
                command.Parameters.AddWithValue("@id_inmueble", contrato.id_inmueble);
                command.Parameters.AddWithValue("@fecha_inicio", contrato.fecha_inicio);
                command.Parameters.AddWithValue("@fecha_fin", contrato.fecha_fin);
                command.Parameters.AddWithValue("@incremento", contrato.incremento);
                command.Parameters.AddWithValue("@estado", contrato.estado);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
