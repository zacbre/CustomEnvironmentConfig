using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Env.Interfaces;

namespace Env.Repositories
{
    public class EnvironmentFileRepository : IEnvironmentVariableRepository
    {
        private readonly Dictionary<string, string> _fileValues = new Dictionary<string, string>();
        private readonly IEnvironmentVariableRepository _environmentVariableRepository = new EnvironmentVariableRepository();
        
        private readonly char[] trimChars = new[] {' ', '"', '\''};
        
        public EnvironmentFileRepository(string fileName, bool requireFile = true)
        {
            ReadFile(fileName, requireFile);
        }
        
        public EnvironmentFileRepository(IEnvironmentVariableRepository environmentVariableRepository, string fileName, bool requireFile = true)
        {
            _environmentVariableRepository = environmentVariableRepository;
            ReadFile(fileName, requireFile);
        }

        public EnvironmentFileRepository(string[] lines)
        {
            ReadLines(lines);
        }
        
        public EnvironmentFileRepository(IEnvironmentVariableRepository environmentVariableRepository, string[] lines)
        {
            _environmentVariableRepository = environmentVariableRepository;
            ReadLines(lines);
        }
        
        public string GetEnvironmentVariable(string keyName)
        {
            var value = _environmentVariableRepository.GetEnvironmentVariable(keyName);
            if (value == null)
            {
                if (_fileValues.TryGetValue(keyName, out value))
                {
                    return value;
                }                
            }

            return value;
        }

        private void ReadLines(string[] lines)
        {
            var currentLine = 0;
            foreach (var line in lines)
            {
                currentLine++;
                ParseLine(line, currentLine);
            }
        }

        private void ReadFile(string fileName, bool requireFile)
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
                ParseLine(line, currentLine);
            }
        }
       
        private void ParseLine(string line, int currentLine)
        {
            if (line.StartsWith("#") || string.IsNullOrEmpty(line))
            {
                return;
            }

            if (!line.Contains("="))
            {
                throw new Exception($"Cannot locate key or value on line {currentLine}. Please include an '=' to indicate key and value.");
            }

            var equalIndex = line.IndexOf("=", StringComparison.Ordinal);
            var keyName = line.Substring(0, equalIndex).Trim(trimChars);
            var keyValue = line.Substring(equalIndex + 1, line.Length - equalIndex - 1).Trim(trimChars);

            if (_fileValues.ContainsKey(keyName))
            {
                throw new DuplicateNameException($"Duplicate key '{keyName}' detected in environment file."); 
            }
            
            _fileValues.Add(keyName, keyValue);
        }
    }
}