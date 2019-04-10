using Env.Interfaces;
using Env.Repositories;
using Env.Tests.Mocks;

namespace Env.Tests
{
    public class BaseTest
    {
        private Mocks.EnvironmentVariableSource _environmentVariableSource;
        public Mocks.EnvironmentVariableSource EnvironmentVariableSource =>
            _environmentVariableSource ?? (_environmentVariableSource = new Mocks.EnvironmentVariableSource());

        private Mocks.FileSource _fileSource;
        public FileSource FileSource => _fileSource ?? (_fileSource = new FileSource());

        private EnvironmentVariableRepository _environmentVariableRepository;
        public EnvironmentVariableRepository EnvironmentVariableRepository =>
            _environmentVariableRepository ?? (_environmentVariableRepository =
                new EnvironmentVariableRepository(EnvironmentVariableSource, FileSource));
        
        private ConfigurationParser _configurationParser;
        public ConfigurationParser ConfigurationParser =>
            _configurationParser ?? (_configurationParser = new ConfigurationParser(EnvironmentVariableRepository));
    }
}