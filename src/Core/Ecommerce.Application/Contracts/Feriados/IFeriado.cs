using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Feriados;

public interface IFeriado
{
    string Insertar(Feriado feriado);
    bool Eliminar(int id);
    Feriado? ObtenerPorId(int id);
    IReadOnlyList<Feriado> Listar();
}
