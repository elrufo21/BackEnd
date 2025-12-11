using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Companias;

public interface ICompania
{
    bool Insertar(Compania compania);
    bool Editar(int id, Compania compania);
    bool Eliminar(int id);
    IReadOnlyList<Compania> Listar();
    IReadOnlyList<EGeneral> ListarCombo();
}
