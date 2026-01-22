using BancoApi.Data;
using BancoApi.DTOs;
using BancoApi.Helpers;
using BancoApi.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterRequestDto dto)
    {
        // Validaciones
        if (!ValidationHelper.IsValidUserName(dto.UserName))
            return (false, "UserName inválido: debe tener 8-20 caracteres, al menos 1 mayúscula, 1 número y sin signos.");

        if (!ValidationHelper.IsValidPassword(dto.Password))
            return (false, "Contraseña inválida: debe tener al menos 8 caracteres, 1 mayúscula, 1 signo y sin espacios.");

        if (!ValidationHelper.IsValidIdentificacion(dto.Identificacion))
            return (false, "Identificación inválida: debe tener 10 dígitos y no contener 4 números repetidos consecutivos.");

        // Verificar duplicados
        if (_context.Usuarios.Any(u => u.UserName == dto.UserName))
            return (false, "El nombre de usuario ya existe.");

        if (_context.Personas.Any(p => p.Identificacion == dto.Identificacion))
            return (false, "La identificación ya está registrada.");

        // Generar correo
        var primeraLetra = dto.Nombres.ToLower().Substring(0, 1);
        var apellidoLimpio = new string(dto.Apellidos.Where(c => char.IsLetter(c)).ToArray()).ToLower();
        var baseMail = $"{primeraLetra}{apellidoLimpio}@mail.com";

        var mailFinal = baseMail;
        var contador = 1;
        while (_context.Usuarios.Any(u => u.Mail == mailFinal))
        {
            mailFinal = $"{primeraLetra}{apellidoLimpio}{contador}@mail.com";
            contador++;
        }

        // Crear Persona
        var persona = new Persona
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Identificacion = dto.Identificacion,
            FechaNacimiento = dto.FechaNacimiento
        };
        _context.Personas.Add(persona);
        await _context.SaveChangesAsync();

        // Crear Usuario
        var usuario = new Usuario
        {
            IdPersona = persona.IdPersona,
            UserName = dto.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Mail = mailFinal,
            Status = "Activo",
            IntentosFallidos = 0
        };
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Asignar Rol
        var usuarioRol = new UsuarioRol
        {
            IdUsuario = usuario.IdUsuario,
            IdRol = dto.IdRol
        };
        _context.UsuarioRoles.Add(usuarioRol);
        await _context.SaveChangesAsync();

        return (true, $"Usuario registrado correctamente. Correo generado: {mailFinal}");
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        // Buscar usuario por UserName o Mail
        var usuario = await _context.Usuarios
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.UserName == dto.Credential || u.Mail == dto.Credential);

        if (usuario == null)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Usuario no encontrado."
            };
        }

        // Si está eliminado lógicamente
        if (usuario.Eliminado)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Usuario inactivo."
            };
        }

        // Si está bloqueado
        if (usuario.Status == "Bloqueado")
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Usuario bloqueado tras 3 intentos fallidos."
            };
        }

        // Verificar si ya tiene una sesión activa
        if (usuario.SessionActive)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = "Ya existe una sesión activa para este usuario."
            };
        }

        // Validar contraseña
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
        {
            // Incrementar intentos fallidos
            usuario.IntentosFallidos++;
            if (usuario.IntentosFallidos >= 3)
            {
                usuario.Status = "Bloqueado";
                usuario.IntentosFallidos = 0; // Opcional: resetear tras bloqueo
            }
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Success = false,
                Message = usuario.Status == "Bloqueado"
                    ? "Usuario bloqueado tras 3 intentos fallidos."
                    : $"Contraseña incorrecta. Intentos restantes: {3 - usuario.IntentosFallidos}"
            };
        }

        // ¡Login exitoso!
        // Resetear intentos fallidos
        usuario.IntentosFallidos = 0;
        usuario.SessionActive = true;
        await _context.SaveChangesAsync();

        // Registrar inicio de sesión
        var session = new Session
        {
            IdUsuario = usuario.IdUsuario,
            FechaIngreso = DateTime.UtcNow,
            Activa = true
        };
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        // Obtener rol (asumimos un solo rol por simplicidad; si hay varios, toma el primero)
        var role = usuario.UsuarioRoles.FirstOrDefault()?.Rol?.NombreRol ?? "Usuario";

        return new LoginResponseDto
        {
            Success = true,
            Message = "Inicio de sesión exitoso.",
            UserName = usuario.UserName,
            Role = role
            // Aquí podrías agregar un token JWT si lo implementas después
        };
    }

    public async Task<bool> LogoutAsync(int idUsuario)
    {
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null || !usuario.SessionActive) return false;

        usuario.SessionActive = false;

        // Cerrar la última sesión activa
        var sessionActiva = await _context.Sessions
            .Where(s => s.IdUsuario == idUsuario && s.Activa)
            .OrderByDescending(s => s.FechaIngreso)
            .FirstOrDefaultAsync();

        if (sessionActiva != null)
        {
            sessionActiva.FechaCierre = DateTime.UtcNow;
            sessionActiva.Activa = false;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}