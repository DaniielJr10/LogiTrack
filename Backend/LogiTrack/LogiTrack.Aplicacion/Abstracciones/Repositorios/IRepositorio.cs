namespace LogiTrack.Aplicacion.Abstracciones.Repositorios;

public interface IRepositorio<TEntidad>
    where TEntidad : class
{
    Task<TEntidad?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntidad>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task AgregarAsync(TEntidad entidad, CancellationToken cancellationToken = default);
    Task ActualizarAsync(TEntidad entidad, CancellationToken cancellationToken = default);
    Task EliminarAsync(Guid id, CancellationToken cancellationToken = default);
}
