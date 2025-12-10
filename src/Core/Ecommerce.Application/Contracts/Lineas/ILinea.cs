using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Lineas;

public interface ILinea
{
    public bool Insertar(Linea linea);
    public bool Eliminar(int id);
    public IReadOnlyList<EGeneral> Listar();
}
