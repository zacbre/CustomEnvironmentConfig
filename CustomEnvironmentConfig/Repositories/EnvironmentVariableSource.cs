using System;
using CustomEnvironmentConfig.Interfaces;

namespace CustomEnvironmentConfig.Repositories
{
    public class EnvironmentVariableSource : IEnvironmentSource
    {
        public string? Get(string keyName)
        {
            return Environment.GetEnvironmentVariable(keyName);
        }
    }
}