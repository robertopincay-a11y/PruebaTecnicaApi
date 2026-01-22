using System.ComponentModel.DataAnnotations;

namespace BancoApi.Models;

public class Persona
{
    [Key]
    public int IdPersona { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public bool Eliminado { get; set; } = false;
}