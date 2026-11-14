namespace LogiTrack.Aplicacion.DTOs.Autenticacion;

public class RestablecerContrasenaSolicitudDto
{
    public string Email { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string NuevaPassword { get; set; } = string.Empty;
}
