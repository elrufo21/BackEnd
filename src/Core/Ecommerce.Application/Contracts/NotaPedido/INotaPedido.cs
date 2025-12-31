using System.Collections.Generic;
using Ecommerce.Domain;
using NotaPedidoEntity = Ecommerce.Domain.NotaPedido;
namespace Ecommerce.Application.Contracts.NotaPedido;
public interface INotaPedido
{
    string RegistrarOrden(string data);
    string EditarOrden(string data);
    string Insertar(NotaPedidoEntity notaPedido);
    string InsertarConDetalle(NotaPedidoEntity notaPedido, IEnumerable<DetalleNota> detalles);
    bool Eliminar(long id);
    NotaPedidoEntity? ObtenerPorId(long id);
    IReadOnlyList<NotaPedidoEntity> ListarCrud(string? estado = null);
    IReadOnlyList<DetalleNota> ListarDetalle(long notaId);
    IReadOnlyList<EListaNota> Listar();
}
