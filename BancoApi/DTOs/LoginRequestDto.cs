namespace BancoApi.DTOs;

public class LoginRequestDto
{
    public string Credential { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
