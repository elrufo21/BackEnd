using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Personales;

public interface IPersonal
{
    string Insertar(Personal personal);
    bool Eliminar(long id);
    IReadOnlyList<Personal> Listar();
}
