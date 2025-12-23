using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Usuarios;

public interface IUsuariosCrud
{
    int Insertar(UsuarioBd usuario);
    bool Editar(int id, UsuarioBd usuario);
    bool Eliminar(int id);
    IReadOnlyList<UsuarioBd> Listar();
    UsuarioBd? ObtenerPorId(int id);
    IReadOnlyList<UsuarioConPersonal> ListarConPersonal();
    UsuarioConPersonal? ObtenerPorIdConPersonal(int id);
}
