using _net_integrador.Models;
using MySql.Data.MySqlClient;

namespace _net_integrador.Repositorios;

public class RepositorioPropietario
{
    string ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";


    //obtener propietarios
    public List<Propietario> ObtenerPropietarios()
    {
        List<Propietario> propietarios = new List<Propietario>();
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            //var query = "SELECT id, nombre, apellido, dni, email, telefono, password, estado FROM propietario";
            var query = "SELECT id, nombre, apellido, dni, email, telefono, estado FROM propietario";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Propietario propietario = new Propietario();
                    propietario.id = reader.GetInt32("id");
                    propietario.nombre = reader.GetString("nombre");
                    propietario.apellido = reader.GetString("apellido");
                    propietario.dni = reader.GetString("dni");
                    propietario.email = reader.GetString("email");
                    propietario.telefono = reader.GetString("telefono");
                    //propietario.password = reader.GetString("password");
                    propietario.estado = reader.GetInt32("estado");
                    propietarios.Add(propietario);
                }
                connection.Close();

            }
            return propietarios;
        }
    }
    public Propietario ObtenerPropietarioId(int id)
    {
        Propietario propietario = new Propietario();
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var sql = "SELECT id, nombre, apellido, dni, email, telefono, estado FROM propietario WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {

                    propietario.id = reader.GetInt32("id");
                    propietario.nombre = reader.GetString("nombre");
                    propietario.apellido = reader.GetString("apellido");
                    propietario.dni = reader.GetString("dni");
                    propietario.email = reader.GetString("email");
                    propietario.telefono = reader.GetString("telefono");
                    //propietario.password = reader.GetString("password");
                    propietario.estado = reader.GetInt32("estado");
                }
                connection.Close();

            }
        }
        return propietario;
    }
    public Propietario ActualizarPropietario(Propietario propietario)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var sql = "UPDATE propietario SET nombre = @nombre, apellido = @apellido, dni = @dni, email = @email, telefono = @telefono WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", propietario.id);
                command.Parameters.AddWithValue("@nombre", propietario.nombre);
                command.Parameters.AddWithValue("@apellido", propietario.apellido);
                command.Parameters.AddWithValue("@dni", propietario.dni);
                command.Parameters.AddWithValue("@email", propietario.email);
                command.Parameters.AddWithValue("@telefono", propietario.telefono);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return propietario;

    }
    public void EliminarPropietario(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var query = "UPDATE propietario SET estado = 0 WHERE id = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    public void AgregarPropietario(Propietario propietario)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            var sql = "INSERT INTO propietario (nombre, apellido, dni, email, telefono, password, estado) VALUES (@nombre, @apellido, @dni, @email, @telefono, @password, @estado)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@nombre", propietario.nombre);
                command.Parameters.AddWithValue("@apellido", propietario.apellido);
                command.Parameters.AddWithValue("@dni", propietario.dni);
                command.Parameters.AddWithValue("@email", propietario.email);
                command.Parameters.AddWithValue("@telefono", propietario.telefono);
                command.Parameters.AddWithValue("@password", propietario.password);
                command.Parameters.AddWithValue("@estado", propietario.estado);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
    
}


    /*
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }
    }*/
