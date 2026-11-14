namespace LogiTrack.Aplicacion;

public sealed class ResultadoAutenticacion
{
    public bool Exito { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public object? Datos { get; set; }

    public static ResultadoAutenticacion Correcto(string mensaje, object? datos = null) =>
        new() { Exito = true, Mensaje = mensaje, Datos = datos };

    public static ResultadoAutenticacion Fallido(string mensaje) =>
        new() { Exito = false, Mensaje = mensaje };
}
