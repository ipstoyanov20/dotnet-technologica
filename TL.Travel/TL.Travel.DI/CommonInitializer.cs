using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TL.AspNet.Security;
using TL.DataAccess.Models;
using TL.Dependency.Injection;
using TL.Travel.DataAccess.Base;
using TL.Travel.Infrastructure;
using TL.Travel.Interfaces;
using DbTLTravelDbContext = TL.Travel.DataAccess.Db.TLTravelDbContext;
using OracleTLTravelDbContext = TL.Travel.DataAccess.Oracle.TLTravelDbContext;

namespace TL.Travel.DI
{
    public static class CommonInitializer
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            return services.AddDomainServices<IService, BaseService>();
        }

        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, DatabaseTypes connectionType, string configuration)
        {
            if (connectionType == DatabaseTypes.Oracle)
            {
                return AddDatabaseContext<OracleTLTravelDbContext>(services, connectionType, configuration);
            }
            else
            {
                return AddDatabaseContext<DbTLTravelDbContext>(services, connectionType, configuration);
            }
        }

        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTLAuthentication<decimal>(configuration)
                    .AddDefaultAuthServices<Operator, BaseTLTravelDbContext>()
                    .AddUserPasswordManager<Operator, BaseTLTravelDbContext>();

            return services;
        }

        public static void AddJwtSecurityScheme(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer Authentication with JWT Token",
                Type = SecuritySchemeType.Http
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddJwtSecurityScheme();
                c.UseInlineDefinitionsForEnums();
                c.IgnoreObsoleteActions();

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "TL Travel Web API",
                    Description = "TL Travel Web API",
                    TermsOfService = new Uri("/terms-of-service", UriKind.Relative),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact Person",
                        Email = string.Empty,
                        Url = new Uri("/contacts", UriKind.Relative),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Licence",
                        Url = new Uri("/licence", UriKind.Relative),
                    }
                });

                c.OrderActionsBy(x => x.GroupName);

                // Add XML comments for better documentation
                try
                {
                    var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
                catch (Exception)
                {
                    // Ignore XML documentation errors - they shouldn't break Swagger generation
                }

                // Handle potential routing conflicts
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            return services;
        }



        private static IServiceCollection AddDatabaseContext<T>(this IServiceCollection services, DatabaseTypes connectionType, string configuration)
            where T : BaseTLTravelDbContext
        {
            services.AddDbContext<T>(options =>
            {
                switch (connectionType)
                {
                    case DatabaseTypes.InMemory:
                        options.UseInMemoryDatabase(configuration ?? "TLTravelDbContext");
                        break;
                    case DatabaseTypes.Oracle:
                        //"Data Source = 172.16.33.50:1521/CRCLRN;User Id = tltravel; Password = tltravel"
                        options.UseOracle(configuration ?? "Data Source = 172.16.40.111:1521/xepdb1;User Id = tltravel_2; Password = tltravel_2;");
                        break;
                    case DatabaseTypes.SQLite:
                        options.UseSqlite(configuration ?? $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TLTravel.db")}");
                        break;
                    case DatabaseTypes.MsSql:
                        options.UseSqlServer(configuration ?? "Data Source=127.0.0.1,1433; Initial Catalog=TL_TRAVEL; User ID=tltravel;Password=tltravel;Trusted_Connection=false;TrustServerCertificate=true; MultipleActiveResultSets=true;");
                        break;
                    default:
                        break;
                }
            }, ServiceLifetime.Scoped, ServiceLifetime.Singleton);

            //services.AddScoped<BaseTLTravelDbContext, OracleTLTravelDbContext>();
            services.AddScoped<BaseTLTravelDbContext, T>();

            return services;
        }

    }
}
