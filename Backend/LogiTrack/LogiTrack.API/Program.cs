using System.Text;
using LogiTrack.Infraestructura.Configuracion;
using LogiTrack.Infraestructura.Inicializacion;
using LogiTrack.Infraestructura.Seguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:5174",
                "https://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var cadenaConexion = builder.Configuration.GetConnectionString("LogiTrack")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión de LogiTrack. Configúrela en User Secrets con la clave ConnectionStrings:LogiTrack.");

builder.Services.AddInfraestructura(cadenaConexion);

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("No se encontró la clave JWT. Configúrela en User Secrets con la clave Jwt:Key.");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"] ?? "LogiTrack",
            ValidAudience = jwtSection["Audience"] ?? "LogiTrack",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministrador", policy => policy.RequireRole(RolesSistema.Administrador));
    options.AddPolicy("RequireCoordinadorLogistico", policy => policy.RequireRole(RolesSistema.CoordinadorLogistico));
    options.AddPolicy("RequireAuxiliarBodega", policy => policy.RequireRole(RolesSistema.AuxiliarBodega));
    options.AddPolicy("RequireConductor", policy => policy.RequireRole(RolesSistema.Conductor));
} );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var inicializador = scope.ServiceProvider.GetRequiredService<InicializadorRoles>();
    await inicializador.InicializarAsync();
}

app.Run();
