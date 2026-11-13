
using LogiTrack.Infraestructura.Configuracion;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Registrar conexión con PostgreSQL
var cadenaConexion = builder.Configuration
    .GetConnectionString("LogiTrack")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión de LogiTrack.");

builder.Services.AddInfraestructura(cadenaConexion);

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
