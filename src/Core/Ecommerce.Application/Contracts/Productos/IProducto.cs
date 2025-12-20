using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Productos;

public interface IProducto
{
    string Insertar(Producto producto);
    bool Eliminar(long id);
    Producto? ObtenerPorId(long id);
    IReadOnlyList<Producto> ListarCrud();
    IReadOnlyList<EListaProducto> Listar();
    IReadOnlyList<EListaProducto> BuscarProducto(string nombre);
}
