using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
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
            for (int i = 0; i < lines.Length; i++)
            {
                var (key, val) = ParseKeyValue(lines[i], i);
                if (key is null || val is null)
                {
                    continue;
                }
                
                val = CheckForMultilineJsonBlock(lines, val, ref i);
                
                AddToStore(key, val);
            }
        }

        private string CheckForMultilineJsonBlock(string[] lines, string val, ref int i)
        {
            var stringBuilder = new StringBuilder(val);
            if (val.StartsWith("`"))
            {
                while (!lines[i++].EndsWith("`"))
                {
                    stringBuilder.Append(lines[i]);
                }
                i--;
            }
            return stringBuilder.ToString().Trim(new [] {'`'});
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

            ReadLines(File.ReadAllLines(fileName));
        }
        
        private (string?, string?) ParseKeyValue(string line, int currentLine)
        {
            if (line.StartsWith("#") || string.IsNullOrEmpty(line))
            {
                return (null, null);
            }

            if (!line.Contains("="))
            {
                throw new Exception($"Cannot locate key or value on line {currentLine}. Please include an '=' to indicate key and value.");
            }

            var equalIndex = line.IndexOf("=", StringComparison.Ordinal);
            var keyName = line.Substring(0, equalIndex).Trim(trimChars);
            var keyValue = line.Substring(equalIndex + 1, line.Length - equalIndex - 1).Trim(trimChars);

            return (keyName, keyValue);
        }
       
        private void AddToStore(string keyName, string keyValue)
        {
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