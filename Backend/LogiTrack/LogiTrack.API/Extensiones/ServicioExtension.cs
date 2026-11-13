namespace LogiTrack.API.Extensiones;

public static class ServicioExtension
{
    public static IServiceCollection AddServiciosApi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services;
    }
}
