using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json;
using CustomEnvironmentConfig.Exceptions;
using CustomEnvironmentConfig.Interfaces;
using CustomEnvironmentConfig.Repositories;

namespace CustomEnvironmentConfig
{
    public class ConfigurationParser
    {
        private readonly IEnvironmentVariableRepository _environmentVariableRepository;

        public ConfigurationParser(IEnvironmentVariableRepository environmentVariableRepository)
        {
            _environmentVariableRepository = environmentVariableRepository;
        }

        public ConfigurationParser(ConfigurationTypeEnum configurationType, string? fileName = null)
        {
            _environmentVariableRepository = new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationType, [fileName]),
                configurationType);
        }
        
        public ConfigurationParser(ConfigurationTypeEnum configurationType, string?[]? fileNames = null)
        {
            _environmentVariableRepository = new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationType, fileNames),
                configurationType);
        }
        
        public static T Parse<T>()
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource()));
            return parser.ParseConfiguration<T>();
        }
        
        public static T ParsePosix<T>()
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource()));
            return parser.ParseConfigurationPosix<T>();
        }
        
        public static T Parse<T>(string fileName)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(ConfigurationTypeEnum.PreferEnvironment, [fileName])));
            return parser.ParseConfiguration<T>();
        }
        
        public static T ParsePosix<T>(string fileName)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(ConfigurationTypeEnum.PreferEnvironment, [fileName])));
            return parser.ParseConfigurationPosix<T>();
        }

        public static T Parse<T>(string fileName, ConfigurationTypeEnum configurationTypeEnum)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, [fileName]),
                configurationTypeEnum));
            return parser.ParseConfiguration<T>();
        }
        
        public static T ParsePosix<T>(string fileName, ConfigurationTypeEnum configurationTypeEnum)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, [fileName]),
                configurationTypeEnum));
            return parser.ParseConfigurationPosix<T>();
        }
        
        public static T Parse<T>(string fileName, ConfigurationTypeEnum configurationTypeEnum, Func<string,string,string> decryptHandler)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, [fileName]),
                configurationTypeEnum));
            return parser.ParseConfiguration<T>(decryptHandler: decryptHandler);
        }
        
        public static T ParsePosix<T>(string fileName, ConfigurationTypeEnum configurationTypeEnum, Func<string,string,string> decryptHandler)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, [fileName]),
                configurationTypeEnum));
            return parser.ParseConfigurationPosix<T>(decryptHandler: decryptHandler, posix: true);
        }

        public static T Parse<T>(IEnvironmentVariableRepository env)
        {
            var parser = new ConfigurationParser(env);
            return parser.ParseConfiguration<T>();
        }
        
        public static T ParsePosix<T>(IEnvironmentVariableRepository env)
        {
            var parser = new ConfigurationParser(env);
            return parser.ParseConfigurationPosix<T>();
        }

        public static T ParseFiles<T>(params string[] fileNames)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(ConfigurationTypeEnum.PreferEnvironment, fileNames)));
            return parser.ParseConfiguration<T>();
        }

        public static T ParseFiles<T>(ConfigurationTypeEnum configurationTypeEnum, params string[] fileNames)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, fileNames), configurationTypeEnum));
            return parser.ParseConfiguration<T>();
        }

        public static T ParseFiles<T>(ConfigurationTypeEnum configurationTypeEnum,
            Func<string, string, string> decryptHandler, params string[] fileNames)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, fileNames), configurationTypeEnum));
            return parser.ParseConfiguration<T>(decryptHandler: decryptHandler);
        }
        
        public static T ParseFilesPosix<T>(params string[] fileNames)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(ConfigurationTypeEnum.PreferEnvironment, fileNames)));
            return parser.ParseConfigurationPosix<T>();
        }

        public static T ParseFilesPosix<T>(ConfigurationTypeEnum configurationTypeEnum, params string[] fileNames)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, fileNames), configurationTypeEnum));
            return parser.ParseConfigurationPosix<T>();
        }

        public static T ParseFilesPosix<T>(ConfigurationTypeEnum configurationTypeEnum,
            Func<string, string, string> decryptHandler, params string[] fileNames)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, fileNames), configurationTypeEnum));
            return parser.ParseConfigurationPosix<T>(decryptHandler: decryptHandler, posix: true);
        }
        
        public T ParseConfiguration<T>()
        {
            return ParseConfiguration<T>(decryptHandler: null);
        }
        
        public T ParseConfiguration<T>(Func<string,string,string>? decryptHandler)
        {
            var type = typeof(T);
            var instance = (T)RuntimeHelpers.GetUninitializedObject(type);
            var requiredProps = new Dictionary<string, RequiredProperty>();
            GetProperties(instance, type, null, requiredProps, new (), decryptHandler, posix: null);
            CheckRequiredProperties(instance, requiredProps);
            return instance;
        }
        
        public T ParseConfigurationPosix<T>()
        {
            return ParseConfigurationPosix<T>(decryptHandler: null, posix: true);
        }
        
        public T ParseConfigurationPosix<T>(Func<string,string,string>? decryptHandler, bool? posix)
        {
            var type = typeof(T);
            var instance = (T)RuntimeHelpers.GetUninitializedObject(type);
            var requiredProps = new Dictionary<string, RequiredProperty>();
            GetProperties(instance, type, null, requiredProps, new (), decryptHandler, posix: posix);
            CheckRequiredProperties(instance, requiredProps);
            return instance;
        }

        private void GetProperties<T>(T instance, Type type, string? prefix, Dictionary<string, RequiredProperty> requiredProps, Stack<Type> recursive, 
                                      Func<string,string,string>? decryptHandler, bool? posix)
        {
            Push(recursive, type);
            
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.MemberType != MemberTypes.Property)
                {
                    continue;
                }

                var itemName = posix is true ? prop.Name.ToUpper() : prop.Name;
                var required = true;
                object? @default = null;
                var jsonDecode = false;
                
                var configItemAttr = prop.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(ConfigurationItem));
                if (configItemAttr is ConfigurationItem cAttr)
                {
                    itemName = posix is true ? cAttr.Name.ToUpper() : cAttr.Name;
                    required = cAttr.Required;
                    @default = cAttr.Default;
                    jsonDecode = cAttr.Json;

                    if (cAttr.Ignore)
                    {
                        continue;
                    }
                }

                var isNullable = Nullable.GetUnderlyingType(prop.PropertyType);

                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType.IsEnum || isNullable is {} || jsonDecode)
                {
                    var val = _environmentVariableRepository.GetEnvironmentVariable(prefix is {} ? $"{prefix + "_" ?? ""}{itemName}" : itemName);
                    switch (required)
                    {
                        case true when string.IsNullOrEmpty(val):
                            requiredProps.TryAdd($"'{(prefix is not null ? $"{prefix + "_" ?? ""}{itemName}" : itemName)}", new RequiredProperty(prefix, itemName, prop, type));
                            break;
                        case false when string.IsNullOrEmpty(val):
                        {
                            if (@default is {})
                            {
                                var conv = Convert.ChangeType(@default, (isNullable ?? prop.PropertyType));
                                prop.SetValue(instance, conv);
                            }
                            continue;
                        }
                    }

                    if (prop.PropertyType.IsEnum)
                    {
                        if (!Enum.TryParse(prop.PropertyType, val, out var parsedEnumVal))
                        {
                            var name = posix is true ? type.Name.ToUpper() : type.Name;
                            var propName = posix is true ? prop.Name.ToUpper() : prop.Name;
                            throw new Exception($"Could not parse '{(prefix is {} ? $"{prefix + "_" ?? ""}{itemName}" : itemName)}' as {name}.{propName}!");
                        }
                        
                        prop.SetValue(instance, parsedEnumVal);
                        continue;
                    }

                    if (jsonDecode && val is {})
                    {
                        var escaped = val.Replace("\\\"", "\"");
                        var deserialized = JsonSerializer.Deserialize(escaped, isNullable ?? prop.PropertyType);
                        prop.SetValue(instance, deserialized);
                        continue;
                    }
                    
                    if (decryptHandler is {} && val is {} && configItemAttr is ConfigurationItem { Encrypt: true })
                    {
                        val = decryptHandler($"{(prefix is { } ? $"{prefix + "_"}{itemName}" : itemName)}", val);
                    }

                    // try to cast env to that type.
                    var convertedVal = Convert.ChangeType(val, (isNullable ?? prop.PropertyType));
                    
                    // set property in instance.
                    prop.SetValue(instance, convertedVal);
                }
                else
                {
                    // Create new instance of type.
                    var subInstance = RuntimeHelpers.GetUninitializedObject(prop.PropertyType);
                    GetProperties(subInstance, prop.PropertyType, $"{(prefix is {} ? prefix + "_" : "")}{itemName}", requiredProps, recursive, decryptHandler, posix: posix);
                    prop.SetValue(instance, subInstance);
                }
            }

            recursive.Pop();
        }

        private void Push(Stack<Type> stack, Type item)
        {
            if (stack.Contains(item))
            {
                throw new RecursiveClassException($"Class instantiation loop detected... Type already initialized: {item.ToString()}");
            }

            stack.Push(item);
        }
        
        private void CheckRequiredProperties<T>(T instance, Dictionary<string, RequiredProperty> requiredProps)
        {
            var missingProps = new List<RequiredProperty>();
            foreach (var (_, reqProp) in requiredProps)
            {
                if (reqProp.Prop.GetValue(instance) is null or "")
                {
                    missingProps.Add(reqProp);
                }
            }
            
            if (missingProps.Any())
            {
                throw new KeyNotFoundException(
                    "[CustomEnvironmentConfig] Required configuration environment variables are empty: " +
                    string.Join(", ", missingProps.Select(p => $"{p.Type.Name}.{p.Prop.Name} (looking for env '{(p.Prefix is not null ? $"{p.Prefix + "_" ?? ""}{p.Name}" : p.Name)}')")));
            }
        }
    }
}