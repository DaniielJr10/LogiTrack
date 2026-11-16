
using LogiTrack.Infraestructura.BaseDatos.Contexto;
using LogiTrack.Aplicacion.Interfaces.Seguridad;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using LogiTrack.Infraestructura.Entidades.Roles;
using LogiTrack.Infraestructura.Entidades.Usuarios;
using LogiTrack.Infraestructura.Inicializacion;
using LogiTrack.Infraestructura.Seguridad;
using LogiTrack.Infraestructura.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogiTrack.Infraestructura.Configuracion;

/// <summary>
/// Extensión para registrar los servicios de infraestructura.
/// </summary>
public static class DependenciasInfraestructura
{
    public static IServiceCollection AddInfraestructura(
        this IServiceCollection services,
        string cadenaConexion)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(cadenaConexion);

        services.AddDbContext<LogiTrackDbContext>(options =>
            options.UseNpgsql(cadenaConexion));

        services.AddIdentity<UsuarioIdentity, RolAplicacion>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<LogiTrackDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ITokenServicio, TokenServicio>();
        services.AddScoped<IUsuarioActualServicio, UsuarioActualServicio>();
        services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
        services.AddScoped<IServicioInventario, ServicioInventario>();
        services.AddScoped<IServicioPedidos, ServicioPedidos>();
        services.AddScoped<IServicioLogistica, ServicioLogistica>();
        services.AddScoped<InicializadorRoles>();

        return services;
    }
}