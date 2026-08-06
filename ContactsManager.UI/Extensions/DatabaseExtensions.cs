using ContactsManager.Core.Domain.Entities;
using ContactsManager.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.UI.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabase(this WebApplication app)
    {
        await app.MigrateDatabase();

        await app.SeedCountriesData();
    }

    public static async Task MigrateDatabase(this WebApplication app)
    {
        var scope = app.Services.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        if (!service.Database.IsRelational())
        {
            return;
        }

        var pendingMigrations = await service.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            await service.Database.MigrateAsync();
        }
    }


    public static async Task SeedCountriesData(this WebApplication app)
    {
        var scope = app.Services.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        if (!service.Database.IsRelational())
        {
            return;
        }

        if (! await service.Countries.AnyAsync())
        {
            var countries = new List<Country>
            {
                new Country { CountryName = "United States" },
                new Country { CountryName = "Canada" },
                new Country { CountryName = "Mexico" },
                new Country { CountryName = "United Kingdom" },
                new Country { CountryName = "Germany" },
                new Country { CountryName = "France" },
            };

            await service.Countries.AddRangeAsync(countries);

            await service.SaveChangesAsync();
        }
    }
}
