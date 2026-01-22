namespace BancoApi.DTOs;

public class RegisterRequestDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int IdRol { get; set; } // Rol asignado al registrar
}