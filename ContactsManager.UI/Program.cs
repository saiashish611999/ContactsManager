using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure;
using ContactsManager.Infrastructure.Repositories;
using ContactsManager.UI.Extensions;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("database");

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentException("No connection string is provided");
}

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// register services
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesService, CountriesService>();

// register repositories
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

var app = builder.Build();

// enabling static files
app.UseStaticFiles();

// initialize database
await app.InitializeDatabase();

// rotativa configuration
if (!app.Environment.IsEnvironment("Test"))
{
    RotativaConfiguration.Setup(app.Environment.WebRootPath, "exe/rotativa");
}


// enable routing
app.UseRouting();

app.MapControllers();

app.Run();
