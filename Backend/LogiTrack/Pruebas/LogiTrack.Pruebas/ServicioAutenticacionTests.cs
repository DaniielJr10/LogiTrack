using System.Security.Claims;
using LogiTrack.Aplicacion.DTOs.Autenticacion;
using LogiTrack.Aplicacion.DTOs.Usuarios;
using LogiTrack.Infraestructura.BaseDatos.Contexto;
using LogiTrack.Infraestructura.Entidades.Roles;
using LogiTrack.Infraestructura.Entidades.Usuarios;
using LogiTrack.Infraestructura.Seguridad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LogiTrack.Pruebas;

public class ServicioAutenticacionTests
{
    [Fact]
    public async Task RegistrarAsync_ConDatosValidos_RetornaToken()
    {
        var (servicio, _, _) = CrearServicio();

        var resultado = await servicio.RegistrarAsync(new RegistroUsuarioSolicitudDto
        {
            Nombres = "Ana",
            Apellidos = "García",
            Email = "ana@logitrack.test",
            Password = "Password123",
            PhoneNumber = "5551234"
        });

        Assert.True(resultado.Exito);
        var token = Assert.IsType<TokenRespuestaDto>(resultado.Datos);
        Assert.False(string.IsNullOrWhiteSpace(token.Token));
        Assert.Equal("ana@logitrack.test", token.Usuario.Email);
    }

    [Fact]
    public async Task RegistrarAsync_ConCorreoDuplicado_DevuelveError()
    {
        var (servicio, _, _) = CrearServicio();

        await servicio.RegistrarAsync(new RegistroUsuarioSolicitudDto
        {
            Nombres = "Ana",
            Apellidos = "García",
            Email = "ana@logitrack.test",
            Password = "Password123"
        });

        var resultadoDuplicado = await servicio.RegistrarAsync(new RegistroUsuarioSolicitudDto
        {
            Nombres = "Otra",
            Apellidos = "Persona",
            Email = "ana@logitrack.test",
            Password = "Password123"
        });

        Assert.False(resultadoDuplicado.Exito);
        Assert.Contains("correo", resultadoDuplicado.Mensaje, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IniciarSesionAsync_ConPasswordIncorrecta_Falla()
    {
        var (servicio, _, _) = CrearServicio();
        await servicio.RegistrarAsync(new RegistroUsuarioSolicitudDto
        {
            Nombres = "Luis",
            Apellidos = "Pérez",
            Email = "luis@logitrack.test",
            Password = "Password123"
        });

        var resultado = await servicio.IniciarSesionAsync(new LoginUsuarioSolicitudDto
        {
            Email = "luis@logitrack.test",
            Password = "PasswordWrong"
        });

        Assert.False(resultado.Exito);
        Assert.Contains("credenciales", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ObtenerUsuariosAsync_SinPermisos_DevuelveError()
    {
        var (servicio, _, httpContextAccessor) = CrearServicio();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, "usuario@logitrack.test")
            }, "TestAuth"))
        };

        var resultado = await servicio.ObtenerUsuariosAsync();

        Assert.False(resultado.Exito);
        Assert.Contains("permisos", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
    }

    private static (ServicioAutenticacion Servicio, UserManager<UsuarioIdentity> UserManager, HttpContextAccessor HttpContextAccessor) CrearServicio()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<LogiTrackDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddIdentity<UsuarioIdentity, RolAplicacion>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<LogiTrackDbContext>()
            .AddDefaultTokenProviders();

        var serviceProvider = services.BuildServiceProvider();
        using (var scope = serviceProvider.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RolAplicacion>>();
            foreach (var rol in RolesSistema.Todos)
            {
                if (!roleManager.RoleExistsAsync(rol).GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new RolAplicacion { Name = rol }).GetAwaiter().GetResult();
                }
            }
        }

        var finalScope = serviceProvider.CreateScope();
        var finalProvider = finalScope.ServiceProvider;
        var userManager = finalProvider.GetRequiredService<UserManager<UsuarioIdentity>>();
        var roleManagerFinal = finalProvider.GetRequiredService<RoleManager<RolAplicacion>>();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        var signInManager = finalProvider.GetRequiredService<SignInManager<UsuarioIdentity>>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "logitrack-super-secret-key-1234567890",
                ["Jwt:Issuer"] = "LogiTrack",
                ["Jwt:Audience"] = "LogiTrack",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        var tokenServicio = new TokenServicio(configuration);
        var usuarioActualServicio = new UsuarioActualServicio(httpContextAccessor);
        var servicio = new ServicioAutenticacion(userManager, signInManager, roleManagerFinal, finalProvider.GetRequiredService<LogiTrackDbContext>(), tokenServicio, usuarioActualServicio);

        return (servicio, userManager, httpContextAccessor);
    }
}
