
using Microsoft.AspNetCore.Identity;

namespace LogiTrack.Dominio.Entidades.Usuarios;

/// <summary>
/// Representa un usuario registrado en LogiTrack.
/// Extiende las funcionalidades de ASP.NET Core Identity.
/// </summary>
public class Usuario : IdentityUser<Guid>
{
    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public bool EstaActivo { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAcceso { get; set; }
}
