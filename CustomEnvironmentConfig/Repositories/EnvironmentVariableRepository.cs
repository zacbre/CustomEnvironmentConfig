using CustomEnvironmentConfig.Interfaces;

namespace CustomEnvironmentConfig.Repositories
{
    public class EnvironmentVariableRepository : IEnvironmentVariableRepository
    {
        private ConfigurationTypeEnum _configurationType;
        private readonly IEnvironmentSource _environmentSource;
        private readonly IEnvironmentSource _fileSource;
               
        public EnvironmentVariableRepository(IEnvironmentSource environmentSource, IEnvironmentSource fileSource, 
            ConfigurationTypeEnum configurationType = ConfigurationTypeEnum.PreferEnvironment)
        {
            _environmentSource = environmentSource;
            _fileSource = fileSource;
            _configurationType = configurationType;
        }
        
        public string? GetEnvironmentVariable(string keyName)
        {
            switch (_configurationType)
            {
                case ConfigurationTypeEnum.PreferEnvironment:
                    return _environmentSource.Get(keyName) ?? _fileSource.Get(keyName);
                case ConfigurationTypeEnum.PreferFile:
                    return _fileSource.Get(keyName) ?? _environmentSource.Get(keyName);
                case ConfigurationTypeEnum.EnvironmentOnly:
                    return _environmentSource.Get(keyName);
                case ConfigurationTypeEnum.FileOnly:
                    return _fileSource.Get(keyName);
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