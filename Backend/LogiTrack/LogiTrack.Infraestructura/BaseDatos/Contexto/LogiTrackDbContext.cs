
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Infraestructura.BaseDatos.Contexto;

/// <summary>
/// Contexto principal de acceso a datos de LogiTrack.
/// Administra la comunicación entre la aplicación y PostgreSQL.
/// </summary>
public class LogiTrackDbContext : DbContext
{
    public LogiTrackDbContext(
        DbContextOptions<LogiTrackDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
