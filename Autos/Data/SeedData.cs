// Data/SeedData.cs
using Autos.Models;
using Microsoft.EntityFrameworkCore;

namespace Autos.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            // Insertar marcas solo si no existen
            if (!context.Marca.Any())
            {
                context.Marca.AddRange(
                    new Marca { Nombre = "Toyota", Descripcion = "Fabricante japonés" },
                    new Marca { Nombre = "Ford", Descripcion = "Fabricante americano" },
                    new Marca { Nombre = "Chevrolet", Descripcion = "Fabricante americano" }
                );
                context.SaveChanges();
            }

            // Insertar modelos si no existen
            if (!context.ModeloCarro.Any())
            {
                var toyotaId = context.Marca
                    .First(m => m.Nombre == "Toyota").MarcaId;

                var fordId = context.Marca
                    .First(m => m.Nombre == "Ford").MarcaId;

                context.ModeloCarro.AddRange(
                    new ModeloCarro
                    {
                        Nombre = "Corolla",
                        Año = 2023,
                        Color = "Blanco",
                        Precio = 25000,
                        Stock = 10,
                        MarcaId = toyotaId
                    },
                    new ModeloCarro
                    {
                        Nombre = "F-150",
                        Año = 2023,
                        Color = "Negro",
                        Precio = 35000,
                        Stock = 5,
                        MarcaId = fordId
                    }
                );
                context.SaveChanges();
            }
        }
    }
}