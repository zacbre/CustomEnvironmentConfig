using System.Collections.Generic;
using CustomEnvironmentConfig.Interfaces;

namespace CustomEnvironmentConfig.Tests.Mocks
{
    public class EnvironmentVariableSource : IEnvironmentSource
    {
        private Dictionary<string, string> _envDict = new();

        public void SetEnvironment(Dictionary<string, string> _dict)
        {
            _envDict = _dict;
        }
        public string? Get(string keyName)
        {
            if (_envDict.ContainsKey(keyName))
            {
                return _envDict[keyName];
            }

            return null;
        }
    }
}