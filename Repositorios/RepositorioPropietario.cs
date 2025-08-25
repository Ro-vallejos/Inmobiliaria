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
}

    /*
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }
    }*/
