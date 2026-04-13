using System.Collections.Generic;
using BusinessEntities;

namespace Ecommerce.Api.Legacy;

public interface ICpeGateway
{
    Dictionary<string, string> Envio(CPE cpe);
    Dictionary<string, string> EnvioResumen(CPE_RESUMEN_BOLETA resumen);
    Dictionary<string, string> EnvioBaja(CPE_BAJA baja);
    Dictionary<string, string> ConsultaTicket(CONSULTA_TICKET consultaTicket);
}
