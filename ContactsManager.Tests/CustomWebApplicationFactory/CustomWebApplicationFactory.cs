using ContactsManager.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsManager.Tests.WebApplicationFactory;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Remove SQL Server registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var databaseName = $"ContactsManagerTestingDatabase_{Guid.NewGuid()}";

            // Register In-Memory database
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={databaseName};Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            });

        });
    }

    public async Task ResetDatabase()
    {
        // Build temporary provider       
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        // Recreate database
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        // Seed data
        await db.Countries.AddRangeAsync(
            new Core.Domain.Entities.Country
            {
                CountryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CountryName = "Israel"
            },
            new Core.Domain.Entities.Country
            {
                CountryId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CountryName = "Iran"
            });

        await db.SaveChangesAsync();

        await db.Persons.AddRangeAsync(
            new Core.Domain.Entities.Person()
            {
                PersonId = Guid.Parse("190F863F-0D26-4FDD-AC24-3B729724C4F8"),
                PersonName = "Sai Ashish",
                EmailAddress = "saiashish611999@gmail.com",
                Gender = Core.Enums.Gender.MALE,
                DateOfBirth = new DateTime(1999, 01, 06),
                Address = "something",
                CountryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                ReceivesNewsLetters = true
            },
            new Core.Domain.Entities.Person()
            {
                PersonId = Guid.Parse("5BC5BAA3-07E3-4309-B15D-3DDC15407E2C"),
                PersonName = "Praveen",
                EmailAddress = "praveen@gmail.com",
                Gender = Core.Enums.Gender.MALE,
                DateOfBirth = new DateTime(1997, 01, 23),
                Address = "something",
                CountryId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ReceivesNewsLetters = true
            });

        db.SaveChanges();
    }
}