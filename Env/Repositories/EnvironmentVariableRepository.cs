using System;
using Env.Interfaces;

namespace Env.Repositories
{
    public class EnvironmentVariableRepository : IEnvironmentVariableRepository
    {
        public string GetEnvironmentVariable(string keyName)
        {
            return Environment.GetEnvironmentVariable(keyName);
        }
    }
}