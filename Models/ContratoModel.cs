using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("contrato")]
    public class Contrato
    {
        public int id { get; set; }
        public int id_inquilino { get; set; }
        public Inquilino Inquilino { get; set; }= new Inquilino ();
        public int id_inmueble { get; set; }
        public Inmueble Inmueble { get; set; }= new Inmueble ();
        public int id_usuario { get; set; }
        public Usuario Usuario { get; set; }= new Usuario ();
        public decimal monto { get; set; }
        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }
        public decimal incremento { get; set; }
        public int estado { get; set; }
    }
}