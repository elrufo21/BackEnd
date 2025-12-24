using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Proveedores;

public interface ICuentaProveedor
{
    string Insertar(CuentaProveedor cuenta);
    bool Eliminar(long cuentaId);
    IReadOnlyList<CuentaProveedor> ListarPorProveedor(long proveedorId);
}
