namespace _net_integrador.Models;

using System.ComponentModel.DataAnnotations;

public class Inquilino 
{
    public int id { get; set; }
    [Required]
    public string? nombre { get; set; }
    [Required]

    public string? apellido { get; set; }
    [Required]

    public string? dni { get; set; }
    [Required, EmailAddress]

    public string? email { get; set; }
    public string? telefono { get; set; }
    public int estado { get; set; }


}