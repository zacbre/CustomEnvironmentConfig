using System;
using Env.Interfaces;

namespace Env.Repositories
{
    public class EnvironmentVariableSource : IEnvironmentSource
    {
        public string Get(string keyName)
        {
            return Environment.GetEnvironmentVariable(keyName);
        }
    }
}