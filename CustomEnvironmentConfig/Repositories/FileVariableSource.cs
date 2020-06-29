using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using CustomEnvironmentConfig.Interfaces;

namespace CustomEnvironmentConfig.Repositories
{
    public class FileVariableSource : IEnvironmentSource
    {
        private readonly Dictionary<string, string> _fileValues = new Dictionary<string, string>();
        private readonly char[] trimChars = new[] {' ', '"', '\''};
        private readonly ConfigurationTypeEnum _configurationType;

        public FileVariableSource()
        {
            _configurationType = ConfigurationTypeEnum.PreferEnvironment;
        }
        public FileVariableSource(ConfigurationTypeEnum configurationType = ConfigurationTypeEnum.PreferEnvironment, string? filename = null)
        {
            _configurationType = configurationType;
            if (filename is {})
            {
                ReadFile(filename);
            }
        }
        
        public FileVariableSource(string[]? lines = null)
        {
            if (lines is {})
            {
                ReadLines(lines);
            }
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

        private void ReadFile(string fileName)
        {
            if (_configurationType == ConfigurationTypeEnum.FileOnly)
            {
                if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                {
                    throw new FileNotFoundException("The environment file you specified was not found.");
                }
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

        public string? Get(string keyName)
        {
            return _fileValues.TryGetValue(keyName, out var value) ? value : null;
        }
    }
}