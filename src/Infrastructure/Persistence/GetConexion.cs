using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence;
public static class GetConexion
{
    public static string GetCadena()
    {
        var builder = WebApplication.CreateBuilder();
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        return connectionString!;
    }  
}
