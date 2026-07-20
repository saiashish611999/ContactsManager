using ContactsManager.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsManager.Tests.WebApplicationFactory;
public sealed class CustomWebApplicationFactory: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            var descriptor = services
            .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<DatabaseContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);                
            }

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
        });
    }
}
