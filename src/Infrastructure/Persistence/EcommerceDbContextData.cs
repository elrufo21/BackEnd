using Ecommerce.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ecommerce.Infrastructure.Persistence;


public class EcommerceDbContextData
{
    public static async Task LoadDataAsync(
        EcommerceDbContext context,
        UserManager<Usuario> usuarioManager,
        //RoleManager<IdentityRole> roleManager,
        ILoggerFactory loggerFactory
    )
    {
        try
        {
            if(!usuarioManager.Users.Any())
            {
                var usuarioAdmin = new Usuario
                {
                    Nombre = "Andre",
                    Apellido = "Ramirez Calla",
                    Email = "scotramirez1612@gmail.com",
                    UserName = "andre1612",
                    Telefono = "924228332",
                    AvatarUrl = "https://firebasestorage.googleapis.com/v0/b/edificacion-app.appspot.com/o/vaxidrez.jpg?alt=media&token=14a28860-d149-461e-9c25-9774d7ac1b24",
                };
                await usuarioManager.CreateAsync(usuarioAdmin, "PasswordJuanPerez123$");
                //await usuarioManager.AddToRoleAsync(usuarioAdmin, Role.ADMIN);

                var usuario = new Usuario
                {
                    Nombre = "Juan",
                    Apellido = "Perez",
                    Email = "juan.perez@gmail.com",
                    UserName = "juan.perez",
                    Telefono = "98563434534",
                    AvatarUrl = "https://firebasestorage.googleapis.com/v0/b/edificacion-app.appspot.com/o/avatar-1.webp?alt=media&token=58da3007-ff21-494d-a85c-25ffa758ff6d",
                };
                await usuarioManager.CreateAsync(usuario, "PasswordJuanPerez123$");
                //await usuarioManager.AddToRoleAsync(usuario, Role.USER);

            }
            
            if (!context.Images!.Any())
            {
                var imageData = File.ReadAllText("../Infrastructure/Data/image.json");
                var imagenes = JsonConvert.DeserializeObject<List<Image>>(imageData);
                await context.Images!.AddRangeAsync(imagenes!);
                await context.SaveChangesAsync();
            }
        
        }
        catch(Exception e)
        {
            var logger = loggerFactory.CreateLogger<EcommerceDbContextData>();
            logger.LogError(e.Message);
        }

    }
    
}
