using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure;
using ContactsManager.Infrastructure.Repositories;
using ContactsManager.UI.Extensions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Rotativa.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// configuring logging
builder.Logging.ClearProviders();

builder.Logging.AddConsole();

if (OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog();
}

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;

    options.RequestBodyLogLimit = 4096;

    options.ResponseBodyLogLimit = 4096;
});

builder.Host.UseSerilog((context, services, logging) =>
{
    logging.ReadFrom.Configuration(context.Configuration)
           .ReadFrom.Services(services)
           .WriteTo.Console();
});

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

// enabling http logging
app.UseHttpLogging();

// enabling static files
app.UseStaticFiles();


// rotativa configuration
if (!app.Environment.IsEnvironment("Test"))
{

    // initialize database
    await app.InitializeDatabase();

    RotativaConfiguration.Setup(app.Environment.WebRootPath, "exe/rotativa");

    ExcelPackage.License.SetNonCommercialPersonal(Guid.NewGuid().ToString());
}


// enable routing
app.UseRouting();

app.MapControllers();

app.Run();


public partial class Program { }