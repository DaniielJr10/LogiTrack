namespace LogiTrack.Aplicacion.DTOs.Autenticacion;

public class ActualizarPerfilSolicitudDto
{
    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
}
