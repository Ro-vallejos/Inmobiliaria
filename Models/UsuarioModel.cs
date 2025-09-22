using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace _net_integrador.Models
{
    [Table("usuario")]
    public class Usuario
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
        
        [Required, DataType(DataType.Password)]

        public string? password { get; set; }
        public string? rol { get; set; }
        public int estado { get; set; }
    }
}