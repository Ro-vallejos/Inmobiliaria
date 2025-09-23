using _net_integrador.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace _net_integrador.Repositorios;

// Ahora la clase RepositorioPago hereda de RepositorioBase e implementa IRepositorioPago
public class RepositorioPago : RepositorioBase, IRepositorioPago
{
    // El constructor ahora recibe la configuración para pasársela a la clase base
    public RepositorioPago(IConfiguration configuration) : base(configuration) { }

    public List<Pago> ObtenerPagosPorContrato(int contratoId)
    {
        List<Pago> pagos = new List<Pago>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT id, id_contrato, fecha_pago, importe, estado FROM pago WHERE id_contrato = @id_contrato";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id_contrato", contratoId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    pagos.Add(new Pago
                    {
                        id = reader.GetInt32("id"),
                        id_contrato = reader.GetInt32("id_contrato"),
                        fecha_pago = reader.GetDateTime("fecha_pago"),
                        importe = reader.GetDecimal("importe"),
                        estado = reader.GetInt32("estado")
                    });
                }
                connection.Close();
            }
        }
        return pagos;
    }
    
    public Pago? ObtenerPagoId(int id)
    {
        Pago? pago = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT id, id_contrato, fecha_pago, importe, estado FROM pago WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    pago = new Pago
                    {
                        id = reader.GetInt32("id"),
                        id_contrato = reader.GetInt32("id_contrato"),
                        fecha_pago = reader.GetDateTime("fecha_pago"),
                        importe = reader.GetDecimal("importe"),
                        estado = reader.GetInt32("estado")
                    };
                }
                connection.Close();
            }
        }
        return pago;
    }
    
    public void AgregarPago(Pago pago)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "INSERT INTO pago (id_contrato, fecha_pago, importe, estado) VALUES (@id_contrato, @fecha_pago, @importe, @estado)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id_contrato", pago.id_contrato);
                command.Parameters.AddWithValue("@fecha_pago", pago.fecha_pago);
                command.Parameters.AddWithValue("@importe", pago.importe);
                command.Parameters.AddWithValue("@estado", pago.estado);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void AnularPago(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE pago SET estado = 0 WHERE id = @id";
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
