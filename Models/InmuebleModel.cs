using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace _net_integrador.Models
{
    [Table("inmueble")]
    public class Inmueble
    {
        public int id { get; set; }
        public int id_propietario { get; set; }
        public Propietario? Propietario { get; set; }
         [Required]

        public string? direccion { get; set; }
         [Required]
        public string? uso { get; set; }
        public int id_tipo { get; set; }
         [Required]

        public TipoInmueble? TipoInmueble { get; set; }
         [Required]

        public int ambientes { get; set; }
         [Required]

        public string? eje_x { get; set; }
         [Required]

        public string? eje_y { get; set; }
         [Required]

        public decimal precio { get; set; }
        public int estado { get; set; }
    }
}