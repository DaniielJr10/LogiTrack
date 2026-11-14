using Microsoft.AspNetCore.Identity;

namespace LogiTrack.Infraestructura.Entidades.Usuarios;

public class UsuarioIdentity : IdentityUser<Guid>
{
    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public bool EstaActivo { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAcceso { get; set; }
}
