using System;

namespace CustomEnvironmentConfig.Exceptions
{
    public class RecursiveClassException : Exception
    {
        private readonly string _message;
        
        public RecursiveClassException(string msg)
        {
            _message = msg;
        }

        public override string ToString()
        {
            return _message;
        }
    }
}