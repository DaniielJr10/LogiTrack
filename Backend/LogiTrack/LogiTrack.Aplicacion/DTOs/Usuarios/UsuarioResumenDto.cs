namespace LogiTrack.Aplicacion.DTOs.Usuarios;

public class UsuarioResumenDto
{
    public Guid Id { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool EstaActivo { get; set; }

    public string RolPrincipal { get; set; } = string.Empty;
}
