namespace BancoApi.DTOs;

public class LoginRequestDto
{
    public string Credential { get; set; } = string.Empty; // Puede ser UserName o Mail
    public string Password { get; set; } = string.Empty;
}