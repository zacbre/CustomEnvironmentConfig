using Env.Interfaces;
using Env.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Env
{
    public static class BuilderExtensions
    {
        public static IWebHostBuilder UseEnvironmentConfiguration<T>(this IWebHostBuilder hostBuilder) where T : class
        {
            return ConfigureService<T>(hostBuilder);
        }
        
        public static IWebHostBuilder UseEnvironmentConfiguration<T>(this IWebHostBuilder hostBuilder, string fileName, bool requireFile = true) where T : class
        {
            return ConfigureService<T>(hostBuilder, fileName, requireFile);
        }

        private static IWebHostBuilder ConfigureService<T>(IWebHostBuilder hostBuilder, string fileName = null, bool requireFile = true)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                IEnvironmentVariableRepository environmentVariableRepository;
                if (fileName != null)
                {
                    environmentVariableRepository = new EnvironmentFileRepository(fileName, requireFile);
                }
                else
                {
                    environmentVariableRepository = new EnvironmentVariableRepository();                    
                }
                
                var configurationParser = new ConfigurationParser(environmentVariableRepository);
                var instance = configurationParser.ParseConfiguration<T>();
                services.AddSingleton(typeof(T), instance);
            });
        }
    }
}