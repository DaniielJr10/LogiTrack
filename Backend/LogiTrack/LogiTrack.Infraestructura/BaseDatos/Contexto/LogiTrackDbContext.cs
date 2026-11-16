using LogiTrack.Dominio.Entidades.Auditoria;
using LogiTrack.Dominio.Entidades.Inventario;
using LogiTrack.Dominio.Entidades.Logistica;
using LogiTrack.Dominio.Entidades.Pedidos;
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
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Almacen> Almacenes { get; set; }
    public DbSet<ExistenciaInventario> ExistenciasInventario { get; set; }
    public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetallePedido> DetallesPedido { get; set; }
    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<Conductor> Conductores { get; set; }
    public DbSet<Ruta> Rutas { get; set; }
    public DbSet<Despacho> Despachos { get; set; }
    public DbSet<Entrega> Entregas { get; set; }
    public DbSet<HistorialSeguimiento> HistorialSeguimientos { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<RegistroAuditoria> RegistrosAuditoria { get; set; }

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
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<RolAplicacion>(entity =>
        {
            entity.ToTable("Roles");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Codigo).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(e => e.PrecioUnitario).HasPrecision(18, 2);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Almacen>(entity =>
        {
            entity.ToTable("Almacenes");
            entity.Property(e => e.Codigo).HasMaxLength(80).IsRequired();
            entity.Property(e => e.Nombre).HasMaxLength(180).IsRequired();
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        modelBuilder.Entity<ExistenciaInventario>(entity =>
        {
            entity.ToTable("ExistenciasInventario");
            entity.HasIndex(e => new { e.ProductoId, e.AlmacenId }).IsUnique();
            entity.Property(e => e.FechaActualizacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.ToTable("MovimientosInventario");
            entity.Property(e => e.FechaMovimiento).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.Property(e => e.NombreCompleto).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.NumeroDocumento).IsUnique();
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedidos");
            entity.Property(e => e.NumeroPedido).HasMaxLength(80).IsRequired();
            entity.HasIndex(e => e.NumeroPedido).IsUnique();
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.CostoEnvio).HasPrecision(18, 2);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.Property(e => e.FechaPedido).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.ToTable("DetallesPedido");
            entity.HasIndex(e => new { e.PedidoId, e.ProductoId }).IsUnique();
            entity.Property(e => e.PrecioUnitario).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.ToTable("Vehiculos");
            entity.Property(e => e.Placa).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Marca).HasMaxLength(80).IsRequired();
            entity.Property(e => e.Modelo).HasMaxLength(80).IsRequired();
            entity.HasIndex(e => e.Placa).IsUnique();
            entity.Property(e => e.CapacidadKg).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Conductor>(entity =>
        {
            entity.ToTable("Conductores");
            entity.Property(e => e.Nombres).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Apellidos).HasMaxLength(120).IsRequired();
            entity.Property(e => e.NumeroDocumento).HasMaxLength(30).IsRequired();
            entity.HasIndex(e => e.NumeroDocumento).IsUnique();
        });

        modelBuilder.Entity<Ruta>(entity =>
        {
            entity.ToTable("Rutas");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Origen).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Destino).HasMaxLength(200).IsRequired();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Despacho>(entity =>
        {
            entity.ToTable("Despachos");
            entity.Property(e => e.Observaciones).HasMaxLength(500);
            entity.Property(e => e.FechaProgramada).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Entrega>(entity =>
        {
            entity.ToTable("Entregas");
            entity.Property(e => e.DireccionEntrega).HasMaxLength(250);
            entity.Property(e => e.CiudadEntrega).HasMaxLength(180);
            entity.Property(e => e.Latitud).HasPrecision(12, 8);
            entity.Property(e => e.Longitud).HasPrecision(12, 8);
            entity.HasIndex(e => new { e.DespachoId, e.PedidoId }).IsUnique();
        });

        modelBuilder.Entity<HistorialSeguimiento>(entity =>
        {
            entity.ToTable("HistorialSeguimiento");
            entity.Property(e => e.Descripcion).HasMaxLength(300).IsRequired();
            entity.Property(e => e.FechaHora).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.ToTable("Notificaciones");
            entity.Property(e => e.Titulo).HasMaxLength(180).IsRequired();
            entity.Property(e => e.Mensaje).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<RegistroAuditoria>(entity =>
        {
            entity.ToTable("RegistrosAuditoria");
            entity.Property(e => e.Accion).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Entidad).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Detalles).HasMaxLength(1000);
            entity.Property(e => e.FechaHora).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("UsuarioClaims"));
        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("UsuarioLogins"));
        modelBuilder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UsuarioTokens"));
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("RolClaims"));
        modelBuilder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UsuarioRoles"));
    }
}
