using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.NotaPedido;
public interface INotaPedido
{
    public string Insertar(String xdata);
    public IReadOnlyList<EListaNota> Listar();
}
