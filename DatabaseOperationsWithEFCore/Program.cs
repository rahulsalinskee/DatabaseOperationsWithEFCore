using DatabaseOperationsWithEFCore.Data;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.CurrencyDTO;
using DatabaseOperationsWithEFCore.Models;
using DatabaseOperationsWithEFCore.Repository.Implementations;
using DatabaseOperationsWithEFCore.Repository.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


/* Registering Db Context For Db Operations using SQL Server and reading connection string from AppSettings.JSON */
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).LogTo(Console.WriteLine);
});

/* Registering Services */
builder.Services.AddScoped<ICurrencyService, CurrencyImplementation>();
builder.Services.AddScoped<IFilterService<Currency>, FilterImplementation<Currency>>();
builder.Services.AddScoped<IBookService, BookImplementation>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
