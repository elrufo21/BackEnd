using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Productos;

public interface IProducto
{
    public IReadOnlyList<EListaProducto> Listar();
    public IReadOnlyList<EListaProducto> BuscarProducto(string nombre);
}
