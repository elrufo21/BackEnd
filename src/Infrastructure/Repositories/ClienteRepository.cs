using Ecommerce.Application.Contracts.Cliente;
using Ecommerce.Domain;

namespace Ecommerce.Infrastructure.Persistence.Repositories;
public class ClienteRepository : ICliente
{
    AccesoDatos daSQL = new AccesoDatos();
    public String ListarCombo()
    {
        String lista=string.Empty;
        //lista = new List<EGeneral>();
        string? rpt;
        rpt = daSQL.ejecutarComando("uspListaComboClienteWeb");
        if (!string.IsNullOrEmpty(rpt))
        {
            lista = rpt; //Cadena. AlistaCampos(rpt);
        }
        return lista;
    }
}
