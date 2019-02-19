using System.Collections.Generic;
using Env.Interfaces;

namespace Env.Tests.Mocks
{
    public class EnvironmentVariableRepository : IEnvironmentVariableRepository
    {
        private Dictionary<string, string> _envDict;

        public EnvironmentVariableRepository()
        {
            _envDict = new Dictionary<string, string>();
        }
        
        public void SetEnvironment(Dictionary<string, string> _dict)
        {
            _envDict = _dict;
        }
        
        public string GetEnvironmentVariable(string keyName)
        {
            if (_envDict.ContainsKey(keyName))
            {
                return _envDict[keyName];
            }

            return null;
        }
    }
}