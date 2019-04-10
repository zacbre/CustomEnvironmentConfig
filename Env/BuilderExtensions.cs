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
        
        public static IWebHostBuilder UseEnvironmentConfiguration<T>(this IWebHostBuilder hostBuilder, string fileName) where T : class
        {
            return ConfigureService<T>(hostBuilder, fileName);
        }

        private static IWebHostBuilder ConfigureService<T>(IWebHostBuilder hostBuilder, string fileName = null, 
            ConfigurationTypeEnum configurationType = ConfigurationTypeEnum.PreferEnvironment)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                var configurationParser = new ConfigurationParser(configurationType, fileName);
                var instance = configurationParser.ParseConfiguration<T>();
                services.AddSingleton(typeof(T), instance);
            });
        }
    }
}