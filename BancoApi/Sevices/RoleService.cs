using BancoApi.Data;
using BancoApi.DTOs;
using BancoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Services;

public class RoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        return await _context.Roles
            .Where(r => !r.Eliminado)
            .Select(r => new RoleDto
            {
                IdRol = r.IdRol,
                NombreRol = r.NombreRol
            })
            .ToListAsync();
    }
}
