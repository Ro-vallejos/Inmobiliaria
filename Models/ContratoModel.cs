using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("contrato")]
    public class Contrato
    {
        public int id { get; set; }
        public int id_inquilino { get; set; }
        public Inquilino? Inquilino { get; set; } 
        [Required(ErrorMessage = "Este campo es obligatorio ")]
        public int id_inmueble { get; set; }
        public Inmueble? Inmueble { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio ")]
        [Range(0, double.MaxValue, ErrorMessage = "Ingrese solo números positivos")]
        public decimal monto_mensual { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Ingrese solo números positivos")]

        public decimal? multa { get; set; }

        public DateTime? fecha_terminacion_anticipada { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio ")]

        public DateTime fecha_inicio { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio ")]

        public DateTime fecha_fin { get; set; }
        public int estado { get; set; }
    }
}