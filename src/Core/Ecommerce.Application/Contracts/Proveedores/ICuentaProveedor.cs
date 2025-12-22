using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Proveedores;

public interface ICuentaProveedor
{
    long Insertar(CuentaProveedor cuenta);
    bool Actualizar(long proveedorId, long cuentaId, CuentaProveedor cuenta);
    bool Eliminar(long proveedorId, long cuentaId);
    IReadOnlyList<CuentaProveedor> ListarPorProveedor(long proveedorId);
}
