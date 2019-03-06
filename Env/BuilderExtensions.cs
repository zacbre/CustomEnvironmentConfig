using Env.Interfaces;
using Env.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Env
{
    public static class BuilderExtensions
    {
        public static IWebHostBuilder UseCustomEnvironmentConfig<T>(this IWebHostBuilder hostBuilder) where T : class
        {
            return ConfigureService<T>(hostBuilder);
        }
        
        public static IWebHostBuilder UseCustomEnvironmentConfig<T>(this IWebHostBuilder hostBuilder, string fileName, bool requireFile = true) where T : class
        {
            return ConfigureService<T>(hostBuilder, fileName, requireFile);
        }

        private static IWebHostBuilder ConfigureService<T>(IWebHostBuilder hostBuilder, string fileName = null, bool requireFile = true)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                if (fileName != null)
                {
                    services.AddTransient<IEnvironmentVariableRepository>(s => new EnvironmentFileRepository(fileName, requireFile));
                }
                else
                {
                    services.AddTransient<IEnvironmentVariableRepository, EnvironmentVariableRepository>();                    
                }
                
                services.AddTransient<ConfigurationParser>();
                var provider = services.BuildServiceProvider();

                var configurationParser = provider.GetService <ConfigurationParser>();

                var instance = configurationParser.ParseConfiguration<T>();
                services.AddSingleton(typeof(T), instance);
            });
        }
    }
}