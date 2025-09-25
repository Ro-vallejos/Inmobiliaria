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
            SELECT 
                c.id, c.id_inquilino, c.id_inmueble, c.fecha_inicio, c.fecha_fin, c.monto_mensual, c.estado AS estadoContrato, c.multa, c.fecha_terminacion_anticipada,
                i.id AS idInquilino, i.nombre AS nombreInquilino, i.apellido AS apellidoInquilino, i.dni AS dniInquilino, 
                i.email AS emailInquilino, i.telefono AS telefonoInquilino, i.estado AS estadoInquilino,
                inm.id AS idInmueble, inm.direccion
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
                        monto_mensual = reader.GetDecimal("monto_mensual"),
                        estado = reader.GetInt32("estadoContrato"),
                        multa = reader.IsDBNull(reader.GetOrdinal("multa")) ? (decimal?)null : reader.GetDecimal("multa"),
                        fecha_terminacion_anticipada = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion_anticipada"))
                                                        ? (DateTime?)null : reader.GetDateTime("fecha_terminacion_anticipada"),

                        Inquilino = new Inquilino
                        {
                            id = reader.GetInt32("idInquilino"),
                            nombre = reader.GetString("nombreInquilino"),
                            apellido = reader.GetString("apellidoInquilino"),
                            dni = reader.GetString("dniInquilino"),
                            email = reader.GetString("emailInquilino"),
                            telefono = reader.GetString("telefonoInquilino"),
                            estado = reader.GetInt32("estadoInquilino")
                        },

                        Inmueble = new Inmueble
                        {
                            id = reader.GetInt32("idInmueble"),
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
            SELECT 
                c.id, c.id_inquilino, c.id_inmueble, c.fecha_inicio, c.fecha_fin, c.monto_mensual, c.estado AS estadoContrato,
                c.multa, c.fecha_terminacion_anticipada,
                i.id AS idInquilino, i.nombre AS nombreInquilino, i.apellido AS apellidoInquilino, i.dni, i.email, i.telefono, i.estado AS estadoInquilino,
                inm.id AS idInmueble, inm.direccion
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
                        monto_mensual = reader.GetDecimal("monto_mensual"),
                        estado = reader.GetInt32("estadoContrato"),
                        multa = reader.IsDBNull(reader.GetOrdinal("multa")) ? (decimal?)null : reader.GetDecimal("multa"),
                        fecha_terminacion_anticipada = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion_anticipada"))
                                                        ? (DateTime?)null : reader.GetDateTime("fecha_terminacion_anticipada"),

                        Inquilino = new Inquilino
                        {
                            id = reader.GetInt32("idInquilino"),
                            nombre = reader.GetString("nombreInquilino"),
                            apellido = reader.GetString("apellidoInquilino"),
                            dni = reader.GetString("dni"),
                            email = reader.GetString("email"),
                            telefono = reader.GetString("telefono"),
                            estado = reader.GetInt32("estadoInquilino")
                        },

                        Inmueble = new Inmueble
                        {
                            id = reader.GetInt32("idInmueble"),
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
            var sql = @"
            INSERT INTO contrato 
                (id_inquilino, id_inmueble, fecha_inicio, fecha_fin, monto_mensual, multa, fecha_terminacion_anticipada, estado)
            VALUES 
                (@id_inquilino, @id_inmueble, @fecha_inicio, @fecha_fin, @monto_mensual, @multa, @fecha_terminacion_anticipada, @estado)";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@id_inquilino", contrato.id_inquilino);
                command.Parameters.AddWithValue("@id_inmueble", contrato.id_inmueble);
                command.Parameters.AddWithValue("@fecha_inicio", contrato.fecha_inicio);
                command.Parameters.AddWithValue("@fecha_fin", contrato.fecha_fin);
                command.Parameters.AddWithValue("@monto_mensual", contrato.monto_mensual);
                command.Parameters.AddWithValue("@multa", DBNull.Value);
                command.Parameters.AddWithValue("@fecha_terminacion_anticipada", DBNull.Value);
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
            var sql = @"
            UPDATE contrato
            SET 
                id_inquilino = @id_inquilino,
                id_inmueble = @id_inmueble,
                fecha_inicio = @fecha_inicio,
                fecha_fin = @fecha_fin,
                monto_mensual = @monto_mensual,
                multa = @multa,
                fecha_terminacion_anticipada = @fecha_terminacion_anticipada,
                estado = @estado
            WHERE id = @id";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@id", contrato.id);
                command.Parameters.AddWithValue("@id_inquilino", contrato.id_inquilino);
                command.Parameters.AddWithValue("@id_inmueble", contrato.id_inmueble);
                command.Parameters.AddWithValue("@fecha_inicio", contrato.fecha_inicio);
                command.Parameters.AddWithValue("@fecha_fin", contrato.fecha_fin);
                command.Parameters.AddWithValue("@monto_mensual", contrato.monto_mensual);
                command.Parameters.AddWithValue("@multa", contrato.multa.HasValue ? contrato.multa.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@fecha_terminacion_anticipada", contrato.fecha_terminacion_anticipada.HasValue ? contrato.fecha_terminacion_anticipada.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@estado", contrato.estado);

                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
    // public List<int> ObtenerInmueblesOcupados(DateTime inicio, DateTime fin)
    // {
    //     var ids = new List<int>();
    //     using (MySqlConnection connection = new MySqlConnection(connectionString))
    //     {
    //         string sql = @"SELECT id_inmueble FROM contrato WHERE NOT (@fin <= fecha_inicio OR @inicio >= fecha_fin) AND (fecha_terminacion_anticipada IS NULL OR fecha_terminacion_anticipada > @inicio) AND estado = 1;";

    //         using (MySqlCommand command = new MySqlCommand(sql, connection))
    //         {
    //             connection.Open();

    //             command.Parameters.AddWithValue("@inicio", inicio);
    //             command.Parameters.AddWithValue("@fin", fin);
    //             var reader = command.ExecuteReader();

    //             while (reader.Read())
    //             {
    //                 ids.Add(reader.GetInt32(0));
    //             }
    //             connection.Close();

    //         }


    //     }
    //     return ids;
    // }

    public List<Contrato> ObtenerContratoPorInmueble(int idInmueble, int idContrato)
    {
         List<Contrato> contratos = new List<Contrato>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = @"SELECT *FROM contrato WHERE id_inmueble = @idInmueble AND id != @idContrato;";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@idInmueble", idInmueble);
                command.Parameters.AddWithValue("@idContrato", idContrato);


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
                        monto_mensual = reader.GetDecimal("monto_mensual"),
                        estado = reader.GetInt32("estado"),
                        multa = reader.IsDBNull(reader.GetOrdinal("multa")) ? (decimal?)null : reader.GetDecimal("multa"),
                        fecha_terminacion_anticipada = reader.IsDBNull(reader.GetOrdinal("fecha_terminacion_anticipada")) ? (DateTime?)null : reader.GetDateTime("fecha_terminacion_anticipada"),
                    });
                }

                connection.Close();
            }
        }

        return contratos;
    }
    
}
