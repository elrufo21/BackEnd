using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Maquinas;

public interface IMaquina
{
    string Insertar(Maquina maquina);
    bool Eliminar(int id);
    IReadOnlyList<Maquina> Listar();
}
