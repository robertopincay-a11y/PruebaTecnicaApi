using System.ComponentModel.DataAnnotations;

namespace BancoApi.Models;

public class Session
{
    [Key]
    public int IdSession { get; set; }
    public int IdUsuario { get; set; }
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public DateTime? FechaCierre { get; set; }
    public bool Activa { get; set; } = true;

    public Usuario Usuario { get; set; } = null!;
}