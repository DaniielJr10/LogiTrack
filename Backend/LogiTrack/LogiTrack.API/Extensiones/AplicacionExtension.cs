namespace LogiTrack.API.Extensiones;

public static class AplicacionExtension
{
    public static WebApplication ConfigurarAplicacion(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app;
    }
}
