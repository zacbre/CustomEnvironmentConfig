using System;

namespace CustomEnvironmentConfig.Exceptions
{
    public class RecursiveClassException(string msg) : Exception
    {
        public override string ToString()
        {
            return msg;
        }
    }
}