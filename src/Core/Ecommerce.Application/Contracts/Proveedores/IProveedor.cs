using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Proveedores;

public interface IProveedor
{
    string Insertar(Proveedor proveedor);
    bool Eliminar(long id);
    Proveedor? ObtenerPorId(long id);
    IReadOnlyList<Proveedor> Listar();
}
