using CustomEnvironmentConfig.Interfaces;

namespace CustomEnvironmentConfig.Repositories
{
    public class EnvironmentVariableRepository(
        IEnvironmentSource environmentSource,
        IEnvironmentSource fileSource,
        ConfigurationTypeEnum configurationType = ConfigurationTypeEnum.PreferEnvironment)
        : IEnvironmentVariableRepository
    {
        private ConfigurationTypeEnum _configurationType = configurationType;

        public string? GetEnvironmentVariable(string keyName)
        {
            switch (_configurationType)
            {
                case ConfigurationTypeEnum.PreferEnvironment:
                    return environmentSource.Get(keyName) ?? fileSource.Get(keyName);
                case ConfigurationTypeEnum.PreferFile:
                    return fileSource.Get(keyName) ?? environmentSource.Get(keyName);
                case ConfigurationTypeEnum.EnvironmentOnly:
                    return environmentSource.Get(keyName);
                case ConfigurationTypeEnum.FileOnly:
                    return fileSource.Get(keyName);
                default:
                    return null;
            }
        }

        public void SetConfigurationType(ConfigurationTypeEnum configurationTypeEnum)
        {
            _configurationType = configurationTypeEnum;
        }
    }
}