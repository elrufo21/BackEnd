using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Personales;

public interface IPersonal
{
    bool Insertar(Personal personal);
    bool Editar(long id, Personal personal);
    bool Eliminar(long id);
    IReadOnlyList<Personal> Listar();
}
