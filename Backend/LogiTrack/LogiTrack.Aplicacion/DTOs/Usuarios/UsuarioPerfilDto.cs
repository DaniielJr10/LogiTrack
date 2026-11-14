namespace LogiTrack.Aplicacion.DTOs.Usuarios;

public class UsuarioPerfilDto
{
    public Guid Id { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool EstaActivo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();
}
