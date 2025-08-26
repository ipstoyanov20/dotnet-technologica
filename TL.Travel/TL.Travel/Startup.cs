using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TL.Common.Settings;
using TL.Travel.DI;
using TL.WebHelpers.Filters;
using TL.WebHelpers.Helpers;
using TL.WebHelpers.ModelBinders.DateBinder;
using TL.WebHelpers.ModelBinders.DateTimeBinder;
using TL.WebHelpers.ModelBinders.Time;

[assembly: ApiController]
namespace TL.TravelAPI
{
    public class Startup
    {
        private const string CORS_POLICY = "AllowSpecificCors";
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(new PathString("/api"));

            app.UseStaticFiles();

            if (env.IsDevelopment() || env.IsStaging() || !env.IsProduction())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger(options =>
                {

                });

                //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                //// specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.DefaultModelExpandDepth(2);
                    c.EnableDeepLinking();
                    c.EnableFilter();
                    c.EnableTryItOutByDefault();
                    c.EnableValidator();
                    c.ShowCommonExtensions();
                });
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(CORS_POLICY);

            app.UseEndpoints(builder =>
            {
                builder.MapControllers();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CORS_POLICY, builder =>
                {
                    builder.AllowAnyHeader();
                    builder.WithOrigins("http://localhost:4200", "http://localhost:5000");
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
            });

            services.AddControllers(configure =>
            {
                configure.AllowEmptyInputInBodyModelBinding = false;
                configure.Filters.Add<TLResultExceptionFilter>();

                TLTimeSpanModelBinderProvider.AddModelBinder(configure);
                TLDateModelBinderProvider.AddModelBinder(configure);
                TLDateTimeModelBinderProvider.ReplaceModelBinder(configure);
            })
            .AddControllersAsServices()
            .AddXmlSerializerFormatters()
            .AddJsonOptions(options =>
            {
                //options.JsonSerializerOptions.Converters.Insert(0, new DateTimeJsonConverter());
                options.JsonSerializerOptions.Converters.Insert(0, new TimeSpanJsonConverter());
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });



            AddDependencyInitializator(services, configuration);
        }

        private static void AddDependencyInitializator(IServiceCollection services, IConfiguration configuration)
        {
            var connectionStrings = ConnectionStrings.ReadSettings(configuration);

            string connectionString = connectionStrings.Connection;

            services.AddDatabaseContext(DatabaseTypes.MsSql, connectionString);
            services.AddCommonServices();
            services.AddSecurity(configuration);
            services.AddSwagger();
        }
    }
}
