using Ecommerce.Application.Contracts.TemporalVenta;
using Ecommerce.Domain;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class TemporalVentaRepository : ITemporalVenta
{
    AccesoDatos daSQL = new AccesoDatos();

    public IReadOnlyList<EListaTemporal> Editar(ETemporalVenta eTemporal)
    {
        List<EListaTemporal> lista;
        lista = new List<EListaTemporal>();
        string? rpt;
        string? xvalue=eTemporal.Cantidad+"|"+ eTemporal.Precio+"|"+eTemporal.Importe+"|"+eTemporal.Id+"|"+eTemporal.UsuarioId;
        rpt = daSQL.ejecutarComando("uspEditarTemWeb","@Data",xvalue);
        if (string.IsNullOrEmpty(rpt))rpt = "error";
        else {lista = Cadena.AlistaCamposTem(rpt);}
        return lista;
    }

    public IReadOnlyList<EListaTemporal> Eliminar(int id)
    {
        List<EListaTemporal> lista;
        lista = new List<EListaTemporal>();
        string? rpt;
        string? xvalue=id+"|3";
        rpt = daSQL.ejecutarComando("uspEliminarTemWeb","@Data",xvalue);
        if (string.IsNullOrEmpty(rpt))rpt = "error";
        else {lista = Cadena.AlistaCamposTem(rpt);}
        return lista;
    }

    public IReadOnlyList<EListaTemporal> EliminarTodo(int id)
    {
        List<EListaTemporal> lista;
        lista = new List<EListaTemporal>();
        string? rpt;
        string? xvalue="3";
        rpt = daSQL.ejecutarComando("uspEliminaTemTodoWeb","@UsuarioId",xvalue);
        if (string.IsNullOrEmpty(rpt))rpt = "error";
        else {lista = Cadena.AlistaCamposTem(rpt);}
        return lista;
    }

    public string Insertar(ETemporalVenta eTemporal)
    {     
        string? xvalue;
        string? rpt;
        xvalue = eTemporal.UsuarioId + "|" + eTemporal.IdProducto + "|" + eTemporal.Cantidad + "|" + eTemporal.Precio + "|" +
                 eTemporal.Importe + "|" + eTemporal.ValorUM + "|" + eTemporal.Unidad + "|" + eTemporal.Codigo;
        rpt = daSQL.ejecutarComando("insertarTempoVentaWeb", "@Data", xvalue);
        if (string.IsNullOrEmpty(rpt))rpt = "error";
        return rpt;
    }

    public IReadOnlyList<EListaTemporal> Listar(int idUser)
    {
        List<EListaTemporal> lista;
        lista = new List<EListaTemporal>();
        string? rpt;
        rpt = daSQL.ejecutarComando("listaTempoVentaWeb","@UsuarioID",idUser.ToString());
        if (!string.IsNullOrEmpty(rpt))
        {
            lista = Cadena.AlistaCamposTem(rpt);
        }
        return lista;
    }
}
