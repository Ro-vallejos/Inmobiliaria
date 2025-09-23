using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace _net_integrador.Models
{
    [Table("inmueble")]
    public class Inmueble
    {
        public int id { get; set; }
        public int id_propietario { get; set; }
        public Propietario Propietario { get; set; } = new Propietario();
        [Required(ErrorMessage = "Este campo es obligatorio")]


        public string direccion { get; set; } = string.Empty;
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public string uso { get; set; } = string.Empty;
        public int id_tipo { get; set; }


        public TipoInmueble TipoInmueble { get; set; } = new TipoInmueble();
        [Required(ErrorMessage = "La cantidad de ambientes es obligatoria")]
        [Range(1, 20, ErrorMessage = "La cantidad de ambientes debe estar entre 1 y 20")]

        public int? ambientes { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public string eje_x { get; set; } = string.Empty;
        [Required(ErrorMessage = "Este campo es obligatorio")]


        public string eje_y { get; set; } = string.Empty;
        [Required(ErrorMessage = "Este campo es obligatorio")]


        public decimal? precio { get; set; } 
        public int estado { get; set; }
    }
}