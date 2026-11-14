using LogiTrack.Infraestructura.Entidades.Roles;
using LogiTrack.Infraestructura.Entidades.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Infraestructura.BaseDatos.Contexto;

/// <summary>
/// Contexto principal de acceso a datos de LogiTrack.
/// Administra la comunicación entre la aplicación y PostgreSQL.
/// </summary>
public class LogiTrackDbContext : IdentityDbContext<UsuarioIdentity, RolAplicacion, Guid>
{
    public LogiTrackDbContext(DbContextOptions<LogiTrackDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsuarioIdentity>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.Property(e => e.Nombres).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Apellidos).HasMaxLength(200).IsRequired();
            entity.Property(e => e.EstaActivo).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<RolAplicacion>(entity =>
        {
            entity.ToTable("Roles");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("UsuarioClaims"));
        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("UsuarioLogins"));
        modelBuilder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UsuarioTokens"));
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("RolClaims"));
        modelBuilder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UsuarioRoles"));
    }
}
