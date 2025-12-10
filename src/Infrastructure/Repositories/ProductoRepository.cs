
using Ecommerce.Application.Contracts.Productos;
using Ecommerce.Domain;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class ProductoRepository : IProducto
{
    AccesoDatos daSQL = new AccesoDatos();
    public IReadOnlyList<EListaProducto> Listar()
    {
        List<EListaProducto> lista;
        lista = new List<EListaProducto>();
        string? rpt;
        rpt = daSQL.ejecutarComando("uspListaWebProducto");
        if (!string.IsNullOrEmpty(rpt))
        {
            lista = Cadena.AlistaCamposPro(rpt);
        }
        return lista;
    }
     public IReadOnlyList<EListaProducto> BuscarProducto(string nombre)
    {
        List<EListaProducto> lista;
        lista = new List<EListaProducto>();
        string? rpt;
        rpt = daSQL.ejecutarComando("uspBuscaWebProducto","@Descripcion", nombre);
        if (!string.IsNullOrEmpty(rpt))
        {
            lista = Cadena.AlistaCamposPro(rpt);
        }
        return lista;
    }
}
