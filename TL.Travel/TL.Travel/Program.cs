using System.Diagnostics;
using TL.Common.Settings;
using TL.Travel.DataAccess.Oracle;
using TL.Travel.Mockups;

namespace TL.TravelAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (IHost host = CreateHostBuilder(args).Build())
            {
#if DEBUG
                CreateDbIfNotExists(host);
#endif
                host.Run();
            }

            Console.WriteLine("Web server stopped!");
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.BuildWebHost();
                            });
        }

        private static IWebHostBuilder BuildWebHost(this IWebHostBuilder webHost)
        {
            return webHost.UseContentRoot(Environment.CurrentDirectory)
                 .ConfigureAppConfiguration((context, builder) =>
                 {
                     builder.SetBasePath(Directory.GetCurrentDirectory());

                     HashSet<string> paths = new HashSet<string>();
                     paths.Add(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'));
                     var fileFullName = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
                     paths.Add(fileFullName.Directory.FullName.TrimEnd('\\'));

                     foreach (var path in paths)
                     {
                         builder.LoadAppSettings(context.HostingEnvironment.EnvironmentName, path);
                     }

                 }).UseStartup<Startup>();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();

                ConnectionStrings connection = ConnectionStrings.ReadSettings(config);

                if (connection.Connection == "in-memory")
                {
                    try
                    {
                        InitializeMockups.Initialize(serviceProvider.GetRequiredService<TLTravelDbContext>());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private static void LoadAppSettings(this IConfigurationBuilder builder, string environment, string location)
        {
            string baseSettingsFile = "appsettings.json";

            builder.AddJsonFile(Path.Combine(location, baseSettingsFile), true);
            builder.AddJsonFile(Path.Combine(location, $"appsettings.{environment}.json"), true);
        }
    }
}
