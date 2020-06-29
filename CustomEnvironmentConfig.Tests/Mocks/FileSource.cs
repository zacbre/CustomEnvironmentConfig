using CustomEnvironmentConfig.Interfaces;
using CustomEnvironmentConfig.Repositories;

namespace CustomEnvironmentConfig.Tests.Mocks
{
    public class FileSource : IEnvironmentSource
    {
        private FileVariableSource _fileVariableSource;

        public FileSource()
        {
            _fileVariableSource = new FileVariableSource();
        }
        
        public void SetEnvironment(string[] lines)
        {
            _fileVariableSource = new FileVariableSource(lines);
        }

        public string? Get(string keyName)
        {
            return _fileVariableSource.Get(keyName);
        }
    }
}