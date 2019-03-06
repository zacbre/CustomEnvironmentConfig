using System;
using System.Collections.Generic;
using System.IO;
using Env.Interfaces;

namespace Env.Repositories
{
    public class EnvironmentFileRepository : IEnvironmentVariableRepository
    {
        private readonly Dictionary<string, string> _fileValues = new Dictionary<string, string>();
        private readonly IEnvironmentVariableRepository _environmentVariableRepository;
        
        public EnvironmentFileRepository(string fileName, bool requireFile = true)
        {
            if (!File.Exists(fileName) && requireFile)
            {
                throw new FileNotFoundException("The environment file you specified was not found.");
            }

            var currentLine = 0;
            var fileLines = File.ReadAllLines(fileName);
            foreach (var line in fileLines)
            {
                currentLine++;
                // Split the string up by first =
                if (line.StartsWith("#"))
                {
                    continue;
                }

                if (!line.Contains("="))
                {
                    throw new Exception($"Cannot locate key or value on line {currentLine}. Please include an '=' to indicate key and value.");
                }

                string keyName = "", keyValue = "";

                var splits = line.Split('=');
                keyName = splits[0].Trim(new char[] { ' ', '"', '\'' });
                for (var i = 1; i < splits.Length; i++)
                {
                    keyValue += splits[i];
                }

                try
                {
                    _fileValues.Add(keyName, keyValue);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Duplicate key '{keyName}' detected in environment file '{fileName}'.");
                }
            }
            
            // Lastly, initialize the environment variable repository just in case we need to fall back to environment.
            _environmentVariableRepository = new EnvironmentVariableRepository();
        }
        public string GetEnvironmentVariable(string keyName)
        {
            // Grab these from a file.
            if (_fileValues.TryGetValue(keyName, out var value))
            {
                return value;
            }
            
            // Try to locate from env.
            return _environmentVariableRepository.GetEnvironmentVariable(keyName);
        }
    }
}