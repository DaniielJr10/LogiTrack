namespace LogiTrack.Aplicacion.DTOs.Autenticacion;

public class TokenRespuestaDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEnUtc { get; set; }

    public UsuarioAutenticadoDto Usuario { get; set; } = new();

    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();
}

public class UsuarioAutenticadoDto
{
    public Guid Id { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
}
