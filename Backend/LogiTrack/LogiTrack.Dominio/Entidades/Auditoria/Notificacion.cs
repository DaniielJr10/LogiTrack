using LogiTrack.Dominio.Enumeraciones;

namespace LogiTrack.Dominio.Entidades.Auditoria;

public class Notificacion
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? UsuarioId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;

    public TipoNotificacion Tipo { get; set; } = TipoNotificacion.Informativa;

    public bool Leida { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
