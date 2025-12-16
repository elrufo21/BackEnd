using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Proveedores;

public interface IProveedor
{
    bool Insertar(Proveedor proveedor);
    bool Editar(long id, Proveedor proveedor);
    bool Eliminar(long id);
    Proveedor? ObtenerPorId(long id);
    IReadOnlyList<Proveedor> Listar();
}
