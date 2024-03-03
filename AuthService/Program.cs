using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using AuthService.Configuration;
using AuthService.Extensions;
using AuthService.Filters;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Initialize;
using AuthService.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// TODO: Serilog perhaps?
// Add serilog
// builder.Host.UseSerilog(
//    (ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Use PostgreSQL with EF Core for database management
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString ?? throw new Exception("Connection string cannot be null"), b => b.MigrationsAssembly("AuthService"));
});


builder.Services.SetupIdentity();

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

// Add fluent validation
//builder.Services.AddFluentValidationAutoValidation();

//builder.Services.AddAutoMapper(typeof(MappingProfile));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Foodify AuthService",
            Version = "v1"
        });
    // using System.Reflection;
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = 
            """
                JWT Authorization header using the Bearer scheme. <br/>
                Enter 'Bearer' [space] and then your token in the text input below.
                <br/> Example: 'Bearer 12345abcdef'
            """,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement 
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

// TODO: Cors perhaps?
/*builder.Services.AddCors(options =>
{
    var globalConfig = builder.Configuration.GetSection("Global");
    options.AddPolicy(name: "Production",
        policy =>
        {
            policy.WithOrigins(
                    globalConfig.GetValue<string>("FrontEndUrl")
                        ?? throw new NullReferenceException("FrontEndUrl cannot be null"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    options.AddPolicy(name: "Development",
        policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true);
        });
});*/

builder.Services.Configure<PasswordHasherOptions>(opt => opt.IterationCount = 210_000);

var app = builder.Build();


/*app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (
        diagnosticContext,
        httpContext) =>
    {
        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
    };
});*/

if (app.Environment.IsDevelopment())
{
    if (args.Contains("db-init") || args.Contains("--db-init"))
    {
        using var scope = app.Services.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        await dbInitializer.Init();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseReDoc(c =>
    {
        c.DocumentTitle = "Auth API";
    });
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
