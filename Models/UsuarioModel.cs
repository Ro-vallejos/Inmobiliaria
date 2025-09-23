using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace _net_integrador.Models
{
    [Table("usuario")]
    public class Usuario
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Ingrese un Nombre")]

        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ingrese un Apellido")]
        public string apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese un DNI")]
        public string dni { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La clave es obligatoria"), DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;
        public string rol { get; set; } = string.Empty;
        public int estado { get; set; }
    }
}