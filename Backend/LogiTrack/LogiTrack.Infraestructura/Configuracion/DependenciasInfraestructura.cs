
using LogiTrack.Infraestructura.BaseDatos.Contexto;
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

        return services;
    }
}
