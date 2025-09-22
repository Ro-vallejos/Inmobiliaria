namespace _net_integrador.Models;
using System.ComponentModel.DataAnnotations;


public class Propietario
{
    public int id { get; set; }
    [Required]
    
    public string? nombre { get; set; }
    [Required]

    public string? apellido { get; set; }
    [Required]

    public string? dni { get; set; }
    [Required,EmailAddress]

    public string? email { get; set; }
    public string? telefono { get; set; }
    [Required(ErrorMessage = "La clave es obligatoria"), DataType(DataType.Password)]

    public string password { get; set; } = "0";
    public int estado { get; set; }
public override string ToString()
		{
			//return $"{Apellido}, {Nombre}";
			//return $"{Nombre} {Apellido}";
			var res = $"{nombre} {apellido}";
			if(!String.IsNullOrEmpty(dni)) {
				res += $" ({dni})";
			}
			return res;
		}

}
//id, nombre, apellido, dni(varchar),email, telegono, password, estado(int)