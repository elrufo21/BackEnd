using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Areas;

public interface IArea
{
    bool Insertar(Area area);
    bool Editar(int id, Area area);
    bool Eliminar(int id);
    IReadOnlyList<EGeneral> Listar();
}
