using Microsoft.EntityFrameworkCore;
using Autos.Models;

namespace Autos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // CORRECCIÓN: Usar singular "Marca" en lugar de plural "Marcas"
        public DbSet<Marca> Marca { get; set; }
        public DbSet<ModeloCarro> ModeloCarro { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Venta> Venta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar nombres de tablas en singular
            modelBuilder.Entity<Marca>().ToTable("Marca");
            modelBuilder.Entity<ModeloCarro>().ToTable("ModeloCarro");
            modelBuilder.Entity<Cliente>().ToTable("Cliente");
            modelBuilder.Entity<Venta>().ToTable("Venta");

            // Configuración de relaciones
            modelBuilder.Entity<ModeloCarro>()
                .HasOne<Marca>()
                .WithMany()
                .HasForeignKey(mc => mc.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne<ModeloCarro>()
                .WithMany()
                .HasForeignKey(v => v.ModeloCarroId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Venta>()
                .HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.NoAction);

            // Restricciones de datos
            modelBuilder.Entity<Marca>()
                .HasIndex(m => m.Nombre)
                .IsUnique();

            modelBuilder.Entity<ModeloCarro>()
                .Property(mc => mc.Stock)
                .HasDefaultValue(0);
            modelBuilder.Entity<ModeloCarro>()
            .Property(mc => mc.Precio)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Venta>()
                .Property(v => v.PrecioVenta)
                .HasColumnType("decimal(18,2)");
        }
    }
}