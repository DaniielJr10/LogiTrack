namespace LogiTrack.Dominio.Excepciones;

public abstract class ExcepcionDominio : Exception
{
    protected ExcepcionDominio(string mensaje)
        : base(mensaje)
    {
    }

    protected ExcepcionDominio(string mensaje, Exception? innerException)
        : base(mensaje, innerException)
    {
    }
}
