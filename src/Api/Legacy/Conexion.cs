namespace MegaRosita.Capa.Aplicacion;

public class Conexion
{
    public string ServidorIP { get; }

    public Conexion()
    {
        var configurado = Environment.GetEnvironmentVariable("CPE_SERVIDOR_IP");
        ServidorIP = string.IsNullOrWhiteSpace(configurado) ? "127.0.0.1" : configurado.Trim();
    }
}
