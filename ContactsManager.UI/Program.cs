using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure;
using ContactsManager.Infrastructure.Repositories;
using ContactsManager.UI.Extensions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

string? connectionString = builder.Configuration.GetConnectionString("Database");

ArgumentNullException.ThrowIfNull(connectionString);

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// services
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

// repositories
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

var app = builder.Build();

app.UseStaticFiles();

if (!app.Environment.IsEnvironment("Test"))
{
    ExcelPackage.License.SetNonCommercialPersonal(Guid.NewGuid().ToString());

    RotativaConfiguration.Setup(app.Environment.WebRootPath, "exe/rotativa");
}
await app.InitializeDatabaseAsync();

app.UseRouting();

app.MapControllers();

app.Run();

public partial class Program { }
