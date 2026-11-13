using Microsoft.Extensions.DependencyInjection;

namespace LogiTrack.Infraestructura.Configuracion;

/// <summary>
/// Extensión para registrar servicios de infraestructura.
/// La implementación real se añadirá cuando se definan los servicios concretos.
/// </summary>
public static class DependenciasInfraestructura
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services;
    }
}
