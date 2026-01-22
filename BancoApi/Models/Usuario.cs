using System.ComponentModel.DataAnnotations;

namespace BancoApi.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }
    public int IdPersona { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public bool SessionActive { get; set; } = false;
    public string Status { get; set; } = "Activo"; // "Activo", "Bloqueado"
    public int IntentosFallidos { get; set; } = 0;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public bool Eliminado { get; set; } = false;

    // Relación
    public Persona Persona { get; set; } = null!;
    public List<UsuarioRol> UsuarioRoles { get; set; } = new();
}