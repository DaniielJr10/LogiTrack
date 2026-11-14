using System.ComponentModel.DataAnnotations;
using LogiTrack.Aplicacion;
using LogiTrack.Aplicacion.DTOs.Autenticacion;
using LogiTrack.Aplicacion.DTOs.Usuarios;
using LogiTrack.Aplicacion.Interfaces.Seguridad;
using LogiTrack.Aplicacion.Interfaces.Servicios;
using LogiTrack.Infraestructura.BaseDatos.Contexto;
using LogiTrack.Infraestructura.Entidades.Roles;
using LogiTrack.Infraestructura.Entidades.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Infraestructura.Seguridad;

public class ServicioAutenticacion : IServicioAutenticacion
{
    private readonly UserManager<UsuarioIdentity> _userManager;
    private readonly SignInManager<UsuarioIdentity> _signInManager;
    private readonly RoleManager<RolAplicacion> _roleManager;
    private readonly LogiTrackDbContext _context;
    private readonly ITokenServicio _tokenServicio;
    private readonly IUsuarioActualServicio _usuarioActualServicio;

    public ServicioAutenticacion(
        UserManager<UsuarioIdentity> userManager,
        SignInManager<UsuarioIdentity> signInManager,
        RoleManager<RolAplicacion> roleManager,
        LogiTrackDbContext context,
        ITokenServicio tokenServicio,
        IUsuarioActualServicio usuarioActualServicio)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
        _tokenServicio = tokenServicio;
        _usuarioActualServicio = usuarioActualServicio;
    }

    public async Task<ResultadoAutenticacion> RegistrarAsync(RegistroUsuarioSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Nombres) || string.IsNullOrWhiteSpace(solicitud.Apellidos))
        {
            return ResultadoAutenticacion.Fallido("Nombres y apellidos son obligatorios.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Email) || !new EmailAddressAttribute().IsValid(solicitud.Email))
        {
            return ResultadoAutenticacion.Fallido("Debe ingresar un correo electrónico válido.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Password) || solicitud.Password.Length < 8)
        {
            return ResultadoAutenticacion.Fallido("La contraseña debe tener al menos 8 caracteres.");
        }

        var usuarioExistente = await _userManager.FindByEmailAsync(solicitud.Email.Trim());
        if (usuarioExistente is not null)
        {
            return ResultadoAutenticacion.Fallido("Ya existe un usuario registrado con ese correo.");
        }

        var nombreUsuario = string.IsNullOrWhiteSpace(solicitud.UserName)
            ? solicitud.Email.Trim()
            : solicitud.UserName.Trim();

        var usuario = new UsuarioIdentity
        {
            UserName = nombreUsuario,
            Email = solicitud.Email.Trim(),
            Nombres = solicitud.Nombres.Trim(),
            Apellidos = solicitud.Apellidos.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(solicitud.PhoneNumber) ? null : solicitud.PhoneNumber.Trim(),
            EmailConfirmed = false,
            EstaActivo = true,
            FechaRegistro = DateTime.UtcNow
        };

        var resultadoCreacion = await _userManager.CreateAsync(usuario, solicitud.Password);
        if (!resultadoCreacion.Succeeded)
        {
            var errores = string.Join("; ", resultadoCreacion.Errors.Select(e => e.Description));
            return ResultadoAutenticacion.Fallido(errores);
        }

        var rolAsignado = RolesSistema.CoordinadorLogistico;
        if (await _roleManager.RoleExistsAsync(rolAsignado))
        {
            await _userManager.AddToRoleAsync(usuario, rolAsignado);
        }

        var respuesta = await CrearRespuestaTokenAsync(usuario, cancellationToken);
        return ResultadoAutenticacion.Correcto("Usuario registrado correctamente.", respuesta);
    }

    public async Task<ResultadoAutenticacion> IniciarSesionAsync(LoginUsuarioSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Email) || string.IsNullOrWhiteSpace(solicitud.Password))
        {
            return ResultadoAutenticacion.Fallido("Correo y contraseña son obligatorios.");
        }

        var usuario = await _userManager.FindByEmailAsync(solicitud.Email.Trim())
            ?? await _userManager.FindByNameAsync(solicitud.Email.Trim());

        if (usuario is null)
        {
            return ResultadoAutenticacion.Fallido("Credenciales inválidas.");
        }

        if (!usuario.EstaActivo)
        {
            return ResultadoAutenticacion.Fallido("La cuenta se encuentra desactivada.");
        }

        var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, solicitud.Password, lockoutOnFailure: true);
        if (!resultado.Succeeded)
        {
            return ResultadoAutenticacion.Fallido(resultado.IsLockedOut
                ? "La cuenta está temporalmente bloqueada por demasiados intentos fallidos."
                : "Credenciales inválidas.");
        }

        usuario.UltimoAcceso = DateTime.UtcNow;
        await _userManager.UpdateAsync(usuario);

        var token = await CrearRespuestaTokenAsync(usuario, cancellationToken);
        return ResultadoAutenticacion.Correcto("Inicio de sesión correcto.", token);
    }

    public async Task<ResultadoAutenticacion> CerrarSesionAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
        return ResultadoAutenticacion.Correcto("Sesión cerrada correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerPerfilAsync(CancellationToken cancellationToken = default)
    {
        var usuarioId = _usuarioActualServicio.ObtenerIdUsuario();
        if (usuarioId is null)
        {
            return ResultadoAutenticacion.Fallido("No se ha identificado un usuario autenticado.");
        }

        var usuario = await _userManager.FindByIdAsync(usuarioId.Value.ToString());
        if (usuario is null)
        {
            return ResultadoAutenticacion.Fallido("Usuario no encontrado.");
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        return ResultadoAutenticacion.Correcto("Perfil obtenido correctamente.", MapearPerfil(usuario, roles));
    }

    public async Task<ResultadoAutenticacion> ActualizarPerfilAsync(ActualizarPerfilSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        var usuarioId = _usuarioActualServicio.ObtenerIdUsuario();
        if (usuarioId is null)
        {
            return ResultadoAutenticacion.Fallido("No se ha identificado un usuario autenticado.");
        }

        var usuario = await _userManager.FindByIdAsync(usuarioId.Value.ToString());
        if (usuario is null)
        {
            return ResultadoAutenticacion.Fallido("Usuario no encontrado.");
        }

        usuario.Nombres = string.IsNullOrWhiteSpace(solicitud.Nombres) ? usuario.Nombres : solicitud.Nombres.Trim();
        usuario.Apellidos = string.IsNullOrWhiteSpace(solicitud.Apellidos) ? usuario.Apellidos : solicitud.Apellidos.Trim();
        usuario.PhoneNumber = string.IsNullOrWhiteSpace(solicitud.PhoneNumber) ? usuario.PhoneNumber : solicitud.PhoneNumber.Trim();

        var resultado = await _userManager.UpdateAsync(usuario);
        if (!resultado.Succeeded)
        {
            return ResultadoAutenticacion.Fallido(string.Join("; ", resultado.Errors.Select(e => e.Description)));
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        return ResultadoAutenticacion.Correcto("Perfil actualizado correctamente.", MapearPerfil(usuario, roles));
    }

    public async Task<ResultadoAutenticacion> RecuperarContrasenaAsync(RecuperarContrasenaSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Email) || !new EmailAddressAttribute().IsValid(solicitud.Email))
        {
            return ResultadoAutenticacion.Fallido("Debe ingresar un correo válido.");
        }

        var usuario = await _userManager.FindByEmailAsync(solicitud.Email.Trim());
        if (usuario is null)
        {
            return ResultadoAutenticacion.Correcto("Si el correo existe, se enviará un enlace para restablecer la contraseña.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
        _ = token;
        return ResultadoAutenticacion.Correcto("Si el correo existe, se enviará un enlace para restablecer la contraseña.");
    }

    public async Task<ResultadoAutenticacion> RestablecerContrasenaAsync(RestablecerContrasenaSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Email) || !new EmailAddressAttribute().IsValid(solicitud.Email))
        {
            return ResultadoAutenticacion.Fallido("Debe ingresar un correo válido.");
        }

        var usuario = await _userManager.FindByEmailAsync(solicitud.Email.Trim());
        if (usuario is null)
        {
            return ResultadoAutenticacion.Fallido("No se encontró una cuenta asociada a ese correo.");
        }

        if (string.IsNullOrWhiteSpace(solicitud.Token) || string.IsNullOrWhiteSpace(solicitud.NuevaPassword) || solicitud.NuevaPassword.Length < 8)
        {
            return ResultadoAutenticacion.Fallido("El token y la nueva contraseña son obligatorios.");
        }

        var resultado = await _userManager.ResetPasswordAsync(usuario, solicitud.Token, solicitud.NuevaPassword);
        if (!resultado.Succeeded)
        {
            return ResultadoAutenticacion.Fallido(string.Join("; ", resultado.Errors.Select(e => e.Description)));
        }

        return ResultadoAutenticacion.Correcto("Contraseña restablecida correctamente.");
    }

    public async Task<ResultadoAutenticacion> ObtenerUsuariosAsync(CancellationToken cancellationToken = default)
    {
        if (!_usuarioActualServicio.TieneRol(RolesSistema.Administrador))
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para consultar usuarios.");
        }

        var usuarios = await _context.Users
            .OrderBy(u => u.Apellidos)
            .ThenBy(u => u.Nombres)
            .ToListAsync(cancellationToken);

        var resultado = new List<UsuarioResumenDto>();
        foreach (var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            var rolPrincipal = roles.FirstOrDefault() ?? "Sin rol";
            resultado.Add(new UsuarioResumenDto
            {
                Id = usuario.Id,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email ?? string.Empty,
                EstaActivo = usuario.EstaActivo,
                RolPrincipal = rolPrincipal
            });
        }

        return ResultadoAutenticacion.Correcto("Lista de usuarios obtenida correctamente.", resultado);
    }

    public async Task<ResultadoAutenticacion> ObtenerUsuarioPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var usuarioActualId = _usuarioActualServicio.ObtenerIdUsuario();
        if (usuarioActualId != id && !_usuarioActualServicio.TieneRol(RolesSistema.Administrador))
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para consultar este usuario.");
        }

        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return ResultadoAutenticacion.Fallido("Usuario no encontrado.");
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        return ResultadoAutenticacion.Correcto("Usuario encontrado.", MapearPerfil(usuario, roles));
    }

    public async Task<ResultadoAutenticacion> ActualizarUsuarioAsync(Guid id, ActualizarUsuarioSolicitudDto solicitud, CancellationToken cancellationToken = default)
    {
        var usuarioActualId = _usuarioActualServicio.ObtenerIdUsuario();
        if (usuarioActualId != id && !_usuarioActualServicio.TieneRol(RolesSistema.Administrador))
        {
            return ResultadoAutenticacion.Fallido("No tiene permisos para modificar este usuario.");
        }

        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return ResultadoAutenticacion.Fallido("Usuario no encontrado.");
        }

        if (!string.IsNullOrWhiteSpace(solicitud.Nombres)) usuario.Nombres = solicitud.Nombres.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.Apellidos)) usuario.Apellidos = solicitud.Apellidos.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.Email)) usuario.Email = solicitud.Email.Trim();
        if (!string.IsNullOrWhiteSpace(solicitud.PhoneNumber)) usuario.PhoneNumber = solicitud.PhoneNumber.Trim();
        if (solicitud.EstaActivo.HasValue) usuario.EstaActivo = solicitud.EstaActivo.Value;

        var resultado = await _userManager.UpdateAsync(usuario);
        if (!resultado.Succeeded)
        {
            return ResultadoAutenticacion.Fallido(string.Join("; ", resultado.Errors.Select(e => e.Description)));
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        return ResultadoAutenticacion.Correcto("Usuario actualizado correctamente.", MapearPerfil(usuario, roles));
    }

    public async Task<ResultadoAutenticacion> CambiarEstadoAsync(Guid id, bool estaActivo, CancellationToken cancellationToken = default)
    {
        if (!_usuarioActualServicio.TieneRol(RolesSistema.Administrador))
        {
            return ResultadoAutenticacion.Fallido("Solo un administrador puede cambiar el estado de las cuentas.");
        }

        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return ResultadoAutenticacion.Fallido("Usuario no encontrado.");
        }

        usuario.EstaActivo = estaActivo;
        var resultado = await _userManager.UpdateAsync(usuario);
        if (!resultado.Succeeded)
        {
            return ResultadoAutenticacion.Fallido(string.Join("; ", resultado.Errors.Select(e => e.Description)));
        }

        return ResultadoAutenticacion.Correcto($"El usuario {usuario.Email} quedó {(estaActivo ? "activo" : "inactivo")}." );
    }

    public async Task<ResultadoAutenticacion> ActualizarRolAsync(Guid id, string rol, CancellationToken cancellationToken = default)
    {
        if (!_usuarioActualServicio.TieneRol(RolesSistema.Administrador))
        {
            return ResultadoAutenticacion.Fallido("Solo un administrador puede administrar roles.");
        }

        if (string.IsNullOrWhiteSpace(rol))
        {
            return ResultadoAutenticacion.Fallido("Debe indicar un rol válido.");
        }

        if (rol == RolesSistema.Administrador)
        {
            return ResultadoAutenticacion.Fallido("No está permitido asignar el rol de administrador desde el registro público.");
        }

        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return ResultadoAutenticacion.Fallido("Usuario no encontrado.");
        }

        if (id == _usuarioActualServicio.ObtenerIdUsuario())
        {
            return ResultadoAutenticacion.Fallido("Un usuario no puede modificar su propio rol.");
        }

        if (!await _roleManager.RoleExistsAsync(rol))
        {
            return ResultadoAutenticacion.Fallido("El rol solicitado no existe en el sistema.");
        }

        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        if (rolesActuales.Count > 0)
        {
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        }

        await _userManager.AddToRoleAsync(usuario, rol);
        return ResultadoAutenticacion.Correcto("Rol actualizado correctamente.");
    }

    private async Task<TokenRespuestaDto> CrearRespuestaTokenAsync(UsuarioIdentity usuario, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(usuario);
        var token = _tokenServicio.GenerarToken(
            usuario.Id.ToString(),
            usuario.Email ?? string.Empty,
            usuario.UserName ?? usuario.Email ?? string.Empty,
            roles);

        return new TokenRespuestaDto
        {
            Token = token,
            ExpiraEnUtc = DateTime.UtcNow.AddMinutes(60),
            Usuario = new UsuarioAutenticadoDto
            {
                Id = usuario.Id,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email ?? string.Empty,
                UserName = usuario.UserName ?? usuario.Email ?? string.Empty
            },
            Roles = roles.ToList()
        };
    }

    private static UsuarioPerfilDto MapearPerfil(UsuarioIdentity usuario, IEnumerable<string> roles)
    {
        return new UsuarioPerfilDto
        {
            Id = usuario.Id,
            Nombres = usuario.Nombres,
            Apellidos = usuario.Apellidos,
            UserName = usuario.UserName ?? string.Empty,
            Email = usuario.Email ?? string.Empty,
            PhoneNumber = usuario.PhoneNumber,
            EstaActivo = usuario.EstaActivo,
            FechaRegistro = usuario.FechaRegistro,
            UltimoAcceso = usuario.UltimoAcceso,
            Roles = roles.ToArray()
        };
    }
}
