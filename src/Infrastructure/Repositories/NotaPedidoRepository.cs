using Ecommerce.Application.Contracts.NotaPedido;
using Ecommerce.Domain;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class NotaPedidoRepository : INotaPedido
{
    AccesoDatos daSQL = new AccesoDatos();
    public string Insertar(String xdata)
    {
        string rpt = string.Empty;
        rpt = daSQL.ejecutarComando("uspinsertarNotaB", "@ListaOrden",xdata);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
    }

    public IReadOnlyList<EListaNota> Listar()
    {
        List<EListaNota> lista;
        lista = new List<EListaNota>();
        string? rpt;
        rpt = daSQL.ejecutarComando("uspListaOrdenWeb");
        if (!string.IsNullOrEmpty(rpt))
        {
            lista = Cadena.AlistaCamposNota(rpt);
        }
        return lista;
    }
}
