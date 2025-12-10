using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.TemporalVenta;

public interface ITemporalVenta
{
    public string Insertar(ETemporalVenta eTemporal);
    public IReadOnlyList<EListaTemporal> Editar(ETemporalVenta eTemporal);
    public IReadOnlyList<EListaTemporal> Listar(int idUser);
    public IReadOnlyList<EListaTemporal> Eliminar(int id);
    public IReadOnlyList<EListaTemporal> EliminarTodo(int id);
}
