using BancoApi.DTOs;
using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var (success, message) = await _authService.RegisterUserAsync(dto);

        if (!success)
        {
            return BadRequest(new { Message = message });
        }

        return Ok(new { Message = message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("logout/{idUsuario}")]
    public async Task<IActionResult> Logout(int idUsuario)
    {
        var success = await _authService.LogoutAsync(idUsuario);
        if (!success)
        {
            return BadRequest(new { Message = "No hay sesión activa para este usuario." });
        }
        return Ok(new { Message = "Sesión cerrada correctamente." });
    }
}