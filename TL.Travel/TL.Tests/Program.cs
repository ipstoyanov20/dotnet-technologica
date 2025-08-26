using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.Travel.DI;

namespace TL.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IServiceProvider serviceProvider = CreateServicesCollection("appsettings.json").BuildServiceProvider();

            Console.WriteLine();

            Console.Read();

        }

        private static IServiceCollection CreateServicesCollection(string jsonFile)
        {
            IServiceCollection services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                  .AddJsonFile(jsonFile, false)
                  .Build();

            services.AddSingleton<IConfiguration>(configuration);

            string connectionString =  configuration.GetConnectionString("Connection");

            services.AddDatabaseContext(DatabaseTypes.MsSql, connectionString);
            services.AddCommonServices();

            return services;
        }
    }
}
