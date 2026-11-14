using LogiTrack.Infraestructura.Entidades.Roles;
using LogiTrack.Infraestructura.Entidades.Usuarios;
using LogiTrack.Infraestructura.Seguridad;
using Microsoft.AspNetCore.Identity;

namespace LogiTrack.Infraestructura.Inicializacion;

public class InicializadorRoles
{
    private readonly RoleManager<RolAplicacion> _roleManager;
    private readonly UserManager<UsuarioIdentity> _userManager;

    public InicializadorRoles(RoleManager<RolAplicacion> roleManager, UserManager<UsuarioIdentity> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task InicializarAsync()
    {
        foreach (var nombreRol in RolesSistema.Todos)
        {
            if (!await _roleManager.RoleExistsAsync(nombreRol))
            {
                await _roleManager.CreateAsync(new RolAplicacion { Name = nombreRol });
            }
        }

        var administradores = await _userManager.GetUsersInRoleAsync(RolesSistema.Administrador);
        if (administradores.Count == 0)
        {
            // La política del backend exige que los usuarios públicos no creen cuentas administrativas.
        }
    }
}
