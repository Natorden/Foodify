using System.Text.Json.Serialization;
using E2EChatApp.Infrastructure.Factories;
using RecipeService.Core.Models.Exceptions;
using RecipeService.Extensions;
using RecipeService.Filters;
using RecipeService.Infrastructure.Factories;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<ExceptionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddProblemDetails();

builder.Services.AddServicesAndRepositories();

// Set up the DB connection
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
{
    var config = builder.Configuration.GetSection("ConnectionStrings");
    return new DbConnectionFactory(config.GetValue<string>("DefaultConnection")
                                   ?? throw new NullReferenceException("Connection string cannot be null"));
});
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();
} else
{
    app.UseCors("Production");
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.UseMiddleware<CurrentContextMiddleware>();

app.Run();
