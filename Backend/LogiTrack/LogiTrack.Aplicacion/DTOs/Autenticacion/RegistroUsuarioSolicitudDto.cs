namespace LogiTrack.Aplicacion.DTOs.Autenticacion;

public class RegistroUsuarioSolicitudDto
{
    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? UserName { get; set; }

    public string Password { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
}
