namespace LogiTrack.Aplicacion.DTOs.Usuarios;

public class ActualizarUsuarioSolicitudDto
{
    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? EstaActivo { get; set; }
}
