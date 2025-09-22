using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("inmueble")]
    public class Inmueble
    {
        public int id { get; set; }
        public int id_propietario { get; set; }
        public Propietario? Propietario { get; set; }
        public string? direccion { get; set; }
        public string? uso { get; set; }
        public int id_tipo { get; set; }
        public TipoInmueble? TipoInmueble { get; set; }
        public int ambientes { get; set; }
        public string? eje_x { get; set; }
        public string? eje_y { get; set; }
        public decimal precio { get; set; }
        public int estado { get; set; }
    }
}