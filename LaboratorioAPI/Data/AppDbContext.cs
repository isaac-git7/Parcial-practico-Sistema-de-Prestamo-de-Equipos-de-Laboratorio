using Microsoft.EntityFrameworkCore;
using LaboratorioAPI.Models;

namespace LaboratorioAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablas que Entity Framework gestionará en la base de datos
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Índices únicos para evitar duplicados en la base de datos
            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Nombre)
                .IsUnique();

            modelBuilder.Entity<Equipo>()
                .HasIndex(e => e.CodigoInventario)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // 2. Control de concurrencia optimista en Equipo mediante RowVersion
            modelBuilder.Entity<Equipo>()
                .Property(e => e.RowVersion)
                .IsRowVersion();

            // 3. Regla de integridad referencial:
            // No permitir borrar un equipo si tiene historial de préstamos (Restrict)
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Equipo)
                .WithMany(e => e.Prestamos)
                .HasForeignKey(p => p.EquipoId)
                .OnDelete(DeleteBehavior.Restrict);

            // No permitir borrar un usuario si tiene préstamos activos o históricos
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Prestamos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Datos semilla iniciales para las Categorías
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    Id = 1,
                    Nombre = "Cómputo Portátil",
                    Descripcion = "Laptops y ultrabooks para prácticas de laboratorio"
                },
                new Categoria
                {
                    Id = 2,
                    Nombre = "Instrumentación y Electrónica",
                    Descripcion = "Osciloscopios, fuentes de poder y generadores de señales"
                },
                new Categoria
                {
                    Id = 3,
                    Nombre = "Microcontroladores e IoT",
                    Descripcion = "Kits Arduino, ESP32, Raspberry Pi y sensores"
                },
                new Categoria
                {
                    Id = 4,
                    Nombre = "Audiovisual",
                    Descripcion = "Proyectores, cámaras y micrófonos para conferencias"
                }
            );
        }
    }
}