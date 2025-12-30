using System.Collections.Generic;
using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Compras;

public interface ICompra
{
    string Insertar(Compra compra);
    string InsertarConDetalle(Compra compra, IEnumerable<DetalleCompra> detalles);
    bool Eliminar(long id);
    Compra? ObtenerPorId(long id);
    IReadOnlyList<Compra> ListarCrud(string? estado = null);
    IReadOnlyList<DetalleCompra> ListarDetalle(long compraId);
}
