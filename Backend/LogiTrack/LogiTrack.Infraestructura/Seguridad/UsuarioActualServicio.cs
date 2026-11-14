using System.Security.Claims;
using LogiTrack.Aplicacion.Interfaces.Seguridad;
using Microsoft.AspNetCore.Http;

namespace LogiTrack.Infraestructura.Seguridad;

public class UsuarioActualServicio : IUsuarioActualServicio
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioActualServicio(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal Usuario => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public Guid? ObtenerIdUsuario()
    {
        var claim = Usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    public string? ObtenerEmail() => Usuario.FindFirst(ClaimTypes.Email)?.Value ?? Usuario.FindFirst("email")?.Value;

    public string? ObtenerUserName() => Usuario.Identity?.Name;

    public bool TieneRol(string rol) => Usuario.IsInRole(rol);
}
