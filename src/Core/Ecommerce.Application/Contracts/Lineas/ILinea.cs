using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Lineas;

public interface ILinea
{
    public string Insertar(Linea linea);
    public bool Editar(int id, Linea linea);
    public bool Eliminar(int id);
    public IReadOnlyList<EGeneral> Listar();
}
