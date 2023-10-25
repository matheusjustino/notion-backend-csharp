namespace NotionBackend.Api.Infrastructure.Startup;

using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NotionBackend.Api.Infrastructure.Configurations;
using NotionBackend.Api.Infrastructure.Persistence;

public static partial class ServiceInitializer
{
    private static IConfiguration _configuration { get; set; }

    private static string _policyName = "CorsPolicy";

    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        _configuration = configuration;

        RegisterDatabase(services);
        RegisterControllers(services);
        RegisterSwagger(services);
        RegisterCustomDependencies(services);
        RegisterCors(services);
        RegisterAutoMapper(services);
        RegisterAuthentication(services);

        return services;
    }

    private static void RegisterDatabase(IServiceCollection services)
    {
        var connectionString = _configuration["ConnectionStrings:DatabaseUrl"];
        if (connectionString is null)
        {
            throw new ApplicationException("Database connection string not found");
        }

        services.AddDbContext<AppDbContext>(
            options =>
            {
                options.UseNpgsql(connectionString);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            },
            ServiceLifetime.Transient);
    }

    private static void RegisterControllers(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
    }

    private static void RegisterCustomDependencies(IServiceCollection services)
    {
        services.AddServices($"{AppDomain.CurrentDomain.FriendlyName}.Api.Application.Services", Assembly.GetExecutingAssembly());
    }

    private static void RegisterSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerDefaultValues>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                },
            });
        });
    }

    private static void RegisterCors(IServiceCollection services)
    {
        var frontendUrl = _configuration["Application:FrontendUrl"];
        if (frontendUrl is null)
        {
            throw new ApplicationException("Cors URL not found");
        }

        services.AddCors(opt => opt.AddPolicy(_policyName, policy =>
            policy
                .SetIsOriginAllowed(_ => true)
                // .WithOrigins(frontendUrl)
                .AllowAnyHeader()
                .AllowAnyMethod()));
    }

    private static void RegisterAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ModelToDTOMapping));
    }

    private static void RegisterAuthentication(IServiceCollection services)
    {
        var secret = _configuration["Application:Secret"];
        if (secret is null)
        {
            throw new ApplicationException("Secret key not defined");
        }

        var key = Encoding.ASCII.GetBytes(secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
    }
}