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

            // Register In-Memory database
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ContactsManagerTestingDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
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
        await db.Database.EnsureCreatedAsync();

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
    }
}