using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Maquinas;

public interface IMaquina
{
    bool Insertar(Maquina maquina);
    bool Editar(int id, Maquina maquina);
    bool Eliminar(int id);
    IReadOnlyList<Maquina> Listar();
}
