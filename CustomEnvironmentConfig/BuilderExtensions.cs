using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CustomEnvironmentConfig
{
    public static class BuilderExtensions
    {
        public static IHostApplicationBuilder UseEnvironmentConfiguration<T>(this IHostApplicationBuilder hostBuilder) where T : class
        {
            return ConfigureService<T>(hostBuilder);
        }
        
        public static IHostApplicationBuilder UseEnvironmentConfiguration<T>(this IHostApplicationBuilder hostBuilder, string? fileName, 
            ConfigurationTypeEnum configurationTypeEnum = ConfigurationTypeEnum.PreferEnvironment) where T : class
        {
            return ConfigureService<T>(hostBuilder, [fileName], configurationTypeEnum);
        }
        
        public static IHostApplicationBuilder UseEnvironmentConfiguration<T>(this IHostApplicationBuilder hostBuilder, 
            ConfigurationTypeEnum configurationTypeEnum = ConfigurationTypeEnum.PreferEnvironment, params string?[]? fileNames) where T : class
        {
            return ConfigureService<T>(hostBuilder, fileNames, configurationTypeEnum);
        }
        
        public static IHostApplicationBuilder UseEnvironmentConfiguration<T>(this IHostApplicationBuilder hostBuilder, T type) where T : class
        {
            hostBuilder.Services.AddSingleton(typeof(T), type);
            return hostBuilder;
        }
        
        public static IHostApplicationBuilder UseEnvironmentConfigurationPosix<T>(this IHostApplicationBuilder hostBuilder) where T : class
        {
            return ConfigureServicePosix<T>(hostBuilder);
        }
        
        public static IHostApplicationBuilder UseEnvironmentConfigurationPosix<T>(this IHostApplicationBuilder hostBuilder, T type) where T : class
        {
            hostBuilder.Services.AddSingleton(typeof(T), type);
            return hostBuilder;
        }
        
        public static IHostApplicationBuilder UseEnvironmentConfigurationPosix<T>(this IHostApplicationBuilder hostBuilder, string? fileName, 
                                                                     ConfigurationTypeEnum configurationTypeEnum = ConfigurationTypeEnum.PreferEnvironment) where T : class
        {
            return ConfigureServicePosix<T>(hostBuilder, [fileName], configurationTypeEnum);
        }
        
        public static IHostApplicationBuilder UseEnvironmentConfigurationPosix<T>(this IHostApplicationBuilder hostBuilder, 
            ConfigurationTypeEnum configurationTypeEnum = ConfigurationTypeEnum.PreferEnvironment, params string?[]? fileNames) where T : class
        {
            return ConfigureServicePosix<T>(hostBuilder, fileNames, configurationTypeEnum);
        }

        private static IHostApplicationBuilder ConfigureService<T>(IHostApplicationBuilder hostBuilder, string?[]? fileNames = null, 
                                                           ConfigurationTypeEnum configurationType = ConfigurationTypeEnum.PreferEnvironment)
        {
            var configurationParser = new ConfigurationParser(configurationType, fileNames);
            var instance = configurationParser.ParseConfiguration<T>();
            if (instance != null) hostBuilder.Services.AddSingleton(typeof(T), instance);
            
            return hostBuilder;
        }
        
        private static IHostApplicationBuilder ConfigureServicePosix<T>(IHostApplicationBuilder hostBuilder, string?[]? fileNames = null, 
                                                           ConfigurationTypeEnum configurationType = ConfigurationTypeEnum.PreferEnvironment)
        {
            var configurationParser = new ConfigurationParser(configurationType, fileNames);
            var instance = configurationParser.ParseConfigurationPosix<T>();
            if (instance != null) hostBuilder.Services.AddSingleton(typeof(T), instance);

            return hostBuilder;
        }
    }
}