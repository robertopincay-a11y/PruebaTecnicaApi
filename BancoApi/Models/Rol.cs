using System.ComponentModel.DataAnnotations;

namespace BancoApi.Models;

public class Rol
{
    [Key]
    public int IdRol { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public bool Eliminado { get; set; } = false;
}