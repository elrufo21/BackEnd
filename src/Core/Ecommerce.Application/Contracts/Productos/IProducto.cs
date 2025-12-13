using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Productos;

public interface IProducto
{
    bool Insertar(Producto producto);
    bool Editar(long id, Producto producto);
    bool Eliminar(long id);
    Producto? ObtenerPorId(long id);
    IReadOnlyList<Producto> ListarCrud();
    IReadOnlyList<EListaProducto> Listar();
    IReadOnlyList<EListaProducto> BuscarProducto(string nombre);
}
