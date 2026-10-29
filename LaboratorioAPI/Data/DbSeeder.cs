using LaboratorioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LaboratorioAPI.Data
{
    public static class DbSeeder
    {
        // Si la tabla Usuarios está vacía, crea un Admin inicial.
        // La contraseña viene de configuración (User Secrets), no del código.
        public static async Task SembrarAdminAsync(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await context.Usuarios.AnyAsync()) return;

            var password = config["Admin:Password"]
                ?? throw new InvalidOperationException("Falta Admin:Password en User Secrets.");

            context.Usuarios.Add(new Usuario
            {
                Username = "admin",
                Email = "admin@laboratorio.local",
                NombreCompleto = "Administrador del Sistema",
                Rol = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            });

            await context.SaveChangesAsync();
        }
    }
}