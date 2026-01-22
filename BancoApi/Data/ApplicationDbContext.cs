using BancoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapeo explícito de nombres de tablas
        modelBuilder.Entity<Persona>().ToTable("Persona");
        modelBuilder.Entity<Usuario>().ToTable("Usuario");
        modelBuilder.Entity<Rol>().ToTable("Rol");
        modelBuilder.Entity<UsuarioRol>().ToTable("UsuarioRol");
        modelBuilder.Entity<Session>().ToTable("Session");

        // Claves compuestas
        modelBuilder.Entity<UsuarioRol>()
            .HasKey(ur => new { ur.IdUsuario, ur.IdRol });

        // Relaciones
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Persona)
            .WithMany()
            .HasForeignKey(u => u.IdPersona);

        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Usuario)
            .WithMany(u => u.UsuarioRoles)
            .HasForeignKey(ur => ur.IdUsuario);

        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Rol)
            .WithMany()
            .HasForeignKey(ur => ur.IdRol);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.Usuario)
            .WithMany()
            .HasForeignKey(s => s.IdUsuario);
    }
}