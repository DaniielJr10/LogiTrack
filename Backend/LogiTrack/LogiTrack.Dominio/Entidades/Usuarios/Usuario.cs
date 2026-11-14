
namespace LogiTrack.Dominio.Entidades.Usuarios;

/// <summary>
/// Representa la entidad de negocio del usuario de LogiTrack.
/// </summary>
public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool EstaActivo { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAcceso { get; set; }
}
