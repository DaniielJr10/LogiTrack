namespace LogiTrack.Aplicacion.Interfaces.Seguridad;

public interface ITokenServicio
{
    string GenerarToken(string userId, string email, string userName, IEnumerable<string> roles);
}
