namespace LogiTrack.Dominio.Entidades.Auditoria;

public class RegistroAuditoria
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? UsuarioId { get; set; }

    public string Accion { get; set; } = string.Empty;

    public string Entidad { get; set; } = string.Empty;

    public string? Detalles { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
}
