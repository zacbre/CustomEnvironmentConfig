using Env.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Env
{
    public static class BuilderExtensions
    {
        public static IWebHostBuilder UseCustomEnvironmentConfig<T>(this IWebHostBuilder hostBuilder) where T : class
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.AddTransient<IEnvironmentVariableRepository, EnvironmentVariableRepository>();
                services.AddTransient<ConfigurationParser>();
                var provider = services.BuildServiceProvider();

                var configurationParser = provider.GetService <ConfigurationParser>();

                var instance = configurationParser.ParseConfiguration<T>();
                services.AddSingleton(typeof(T), instance);
            });
        }
    }
}