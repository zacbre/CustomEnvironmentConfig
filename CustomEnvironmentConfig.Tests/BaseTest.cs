using CustomEnvironmentConfig.Repositories;
using CustomEnvironmentConfig.Tests.Mocks;
using EnvironmentVariableSource = CustomEnvironmentConfig.Tests.Mocks.EnvironmentVariableSource;

namespace CustomEnvironmentConfig.Tests
{
    public class BaseTest
    {
        private EnvironmentVariableSource? _environmentVariableSource;
        public EnvironmentVariableSource EnvironmentVariableSource =>
            _environmentVariableSource ??= new EnvironmentVariableSource();

        private FileSource? _fileSource;
        public FileSource FileSource => _fileSource ??= new FileSource();

        private EnvironmentVariableRepository? _environmentVariableRepository;
        public EnvironmentVariableRepository EnvironmentVariableRepository =>
            _environmentVariableRepository ??= new EnvironmentVariableRepository(EnvironmentVariableSource, FileSource);
        
        private ConfigurationParser? _configurationParser;
        public ConfigurationParser ConfigurationParser =>
            _configurationParser ??= new ConfigurationParser(EnvironmentVariableRepository);
    }
}