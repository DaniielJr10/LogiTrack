namespace LogiTrack.Infraestructura.Seguridad;

public static class RolesSistema
{
    public const string Administrador = "Administrador";
    public const string CoordinadorLogistico = "CoordinadorLogistico";
    public const string AuxiliarBodega = "AuxiliarBodega";
    public const string Conductor = "Conductor";

    public static readonly string[] Todos =
    [
        Administrador,
        CoordinadorLogistico,
        AuxiliarBodega,
        Conductor
    ];
}
