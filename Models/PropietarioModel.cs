namespace _net_integrador.Models;

public class Propietario
{
    public int id { get; set; }
    public string? nombre { get; set; }
    public string? apellido { get; set; }
    public string? dni { get; set; }
    public string? email { get; set; }
    public string? telefono { get; set; }
    public string password { get; set; } = "0";
    public int estado { get; set; }


}
//id, nombre, apellido, dni(varchar),email, telegono, password, estado(int)