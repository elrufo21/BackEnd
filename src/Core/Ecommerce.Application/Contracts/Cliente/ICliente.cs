using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Clientes;

public interface ICliente
{
    bool Insertar(Cliente cliente);
    bool Editar(long id, Cliente cliente);
    bool Eliminar(long id);
    IReadOnlyList<Cliente> Listar();
    string ListarCombo();
}
