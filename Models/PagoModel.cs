using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace _net_integrador.Models
{
    [Table("pago")]
    public class Pago
    {
        public int id { get; set; }
        public int id_contrato { get; set; }
        public Contrato Contrato { get; set; }= new Contrato();    
        public DateTime fecha_pago { get; set; }        
        public decimal importe { get; set; }
        public int estado { get; set; }
    }
}