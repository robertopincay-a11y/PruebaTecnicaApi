namespace BancoApi.DTOs;

public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; } // Opcional si usas JWT más adelante
    public string? UserName { get; set; }
    public string? Role { get; set; }
}