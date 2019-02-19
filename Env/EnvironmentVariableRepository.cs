using System;
using Env.Interfaces;

namespace Env
{
    public class EnvironmentVariableRepository : IEnvironmentVariableRepository
    {
        public string GetEnvironmentVariable(string keyName)
        {
            return Environment.GetEnvironmentVariable(keyName);
        }
    }
}