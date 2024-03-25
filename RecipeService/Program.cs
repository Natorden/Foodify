using System.Text;
using System.Text.Json.Serialization;
using E2EChatApp.Infrastructure.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RecipeService.Extensions;
using RecipeService.Filters;
using RecipeService.Infrastructure.Factories;
using RecipeService.Middleware;
using Shared;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure gRPC
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<Profile.ProfileClient>(o => 
    o.Address = new Uri(
        builder.Configuration.GetSection("Grpc").GetValue<string>("ProfileAddress")
        ?? throw new NullReferenceException("ProfileAddress cannot be null")
    )
);
builder.Services.AddGrpcClient<Comment.CommentClient>(o => 
    o.Address = new Uri(
        builder.Configuration.GetSection("Grpc").GetValue<string>("CommentAddress")
        ?? throw new NullReferenceException("CommentAddress cannot be null")
    )
);

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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtConfig = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtConfig.GetValue<string>("Key")
                                         ?? throw new NullReferenceException("JWT key cannot be null"));
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.GetValue<string>("Issuer"),
            ValidAudience = jwtConfig.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

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

app.UseMiddleware<CurrentContextMiddleware>();

app.Run();
