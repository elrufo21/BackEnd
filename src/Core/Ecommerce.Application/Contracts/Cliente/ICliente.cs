using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Clientes;

public interface ICliente
{
    string Insertar(Cliente cliente);
    bool Eliminar(long id);
    IReadOnlyList<Cliente> Listar(string? estado = "ACTIVO");
    string ListarCombo();
}
