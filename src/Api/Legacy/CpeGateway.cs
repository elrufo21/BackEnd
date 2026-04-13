using System.Collections.Generic;
using BusinessEntities;
using MegaRosita.Capa.Aplicacion;

namespace Ecommerce.Api.Legacy;

public sealed class CpeGateway : ICpeGateway
{
    public Dictionary<string, string> Envio(CPE cpe)
    {
        return new CPEConfig().Envio(cpe);
    }

    public Dictionary<string, string> EnvioResumen(CPE_RESUMEN_BOLETA resumen)
    {
        return new CPEConfig().EnvioResumen(resumen);
    }

    public Dictionary<string, string> EnvioBaja(CPE_BAJA baja)
    {
        return new CPEConfig().EnvioBaja(baja);
    }

    public Dictionary<string, string> ConsultaTicket(CONSULTA_TICKET consultaTicket)
    {
        return new CPEConfig().ConsultaTicket(consultaTicket);
    }
}
