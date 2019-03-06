using Env.Interfaces;
using Env.Repositories;

namespace Env.Tests
{
    public class BaseTest
    {
        private Mocks.EnvironmentVariableRepository _environmentVariableRepository;
        public Mocks.EnvironmentVariableRepository EnvironmentVariableRepository =>
            _environmentVariableRepository ?? (_environmentVariableRepository = new Mocks.EnvironmentVariableRepository());

        private ConfigurationParser _configurationParser;

        public ConfigurationParser ConfigurationParser =>
            _configurationParser ?? (_configurationParser = new ConfigurationParser(EnvironmentVariableRepository));
    }
}