using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.Travel.DataAccess.Base;
using TL.Travel.DI;
using TL.Travel.Mockups;

namespace TL.UnitTests
{
    public abstract class BaseUnitTests
    {
        protected IServiceProvider serviceProvider;
        public BaseUnitTests()
        {
            serviceProvider = CreateServicesCollection("appsettings.json").BuildServiceProvider();

            var dbContext = serviceProvider.GetService<BaseTLTravelDbContext>();
            InitializeMockups.Initialize(dbContext);
        }

        private static IServiceCollection CreateServicesCollection(string jsonFile)
        {
            IServiceCollection services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                  .AddJsonFile(jsonFile, false)
                  .Build();

            services.AddSingleton<IConfiguration>(configuration);

            string connectionString = null;// configuration.GetConnectionString("Connection");

            services.AddDatabaseContext(DatabaseTypes.InMemory, connectionString);
            services.AddCommonServices();

            return services;
        }
    }
}
