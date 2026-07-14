using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure;
using ContactsManager.Infrastructure.Repositories;
using ContactsManager.UI.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("Database");

ArgumentNullException.ThrowIfNull(connectionString);

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// services
builder.Services.AddScoped<ICountriesService, CountriesService>();

// repositories
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.MapGet("/", () => "Hello World!");

app.Run();
