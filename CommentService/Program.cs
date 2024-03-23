using System.Text;
using System.Text.Json.Serialization;
using CommentService.Converters;
using CommentService.Extensions;
using CommentService.Filters;
using CommentService.Infrastructure.Factories;
using CommentService.Infrastructure.RpcServices;
using CommentService.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Shared;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure gRPC
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<Profile.ProfileClient>(o => 
    o.Address = new Uri(
        builder.Configuration.GetSection("Grpc").GetValue<string>("ProfileAddress")
        ?? throw new NullReferenceException("JWT key cannot be null")
    )
);

// Split HTTP1 (REST) and HTTP2 (gRPC) to different ports
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http1);
    // Setup a HTTP/2 endpoint without TLS.
    options.ListenAnyIP(50051, o => o.Protocols = HttpProtocols.Http2);
});

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

app.MapGrpcService<CommentRpcService>();

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
