using ContactsManager.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("database");

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentException("No connection string is provided");
}

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
