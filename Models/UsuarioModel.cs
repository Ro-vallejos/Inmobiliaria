using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace _net_integrador.Models
{
    [Table("usuario")]
    public class Usuario
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Ingrese un Nombre")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "El nombre no puede contener números ni caracteres especiales")]

        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ingrese un Apellido")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "El apellido no puede contener números ni caracteres especiales")]

        public string apellido { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ingrese un DNI")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI no es válido")]

        public string dni { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El telefono es obligatorio")]
        [RegularExpression(@"^(\d{8,12})?$", ErrorMessage = "El teléfono no es válido")]
        public string telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La clave es obligatoria"), DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,12}$", ErrorMessage = "La contraseña debe tener entre 8 y 12 caracteres, al menos una mayúscula, un número y un signo")]

        public string password { get; set; } = string.Empty;
        public string rol { get; set; } = string.Empty;
        public int estado { get; set; }
    }
}