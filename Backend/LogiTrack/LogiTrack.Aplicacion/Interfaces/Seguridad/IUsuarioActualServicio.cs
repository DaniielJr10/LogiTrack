namespace LogiTrack.Aplicacion.Interfaces.Seguridad;

public interface IUsuarioActualServicio
{
    Guid? ObtenerIdUsuario();

    string? ObtenerEmail();

    string? ObtenerUserName();

    bool TieneRol(string rol);
}
