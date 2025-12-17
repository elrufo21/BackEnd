using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Areas;

public interface IArea
{
    string Insertar(Area area);
    bool Eliminar(int id);
    IReadOnlyList<EGeneral> Listar();
}
