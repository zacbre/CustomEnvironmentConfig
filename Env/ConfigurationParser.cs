using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Env.Exceptions;
using Env.Interfaces;
using Env.Repositories;

namespace Env
{
    public class ConfigurationParser
    {
        private readonly IEnvironmentVariableRepository _environmentVariableRepository;

        public ConfigurationParser(IEnvironmentVariableRepository environmentVariableRepository)
        {
            _environmentVariableRepository = environmentVariableRepository;
        }

        public ConfigurationParser(ConfigurationTypeEnum configurationType, string fileName = null)
        {
            _environmentVariableRepository = new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationType, fileName),
                configurationType);
        }
        
        public static T Parse<T>()
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource()));
            return parser.ParseConfiguration<T>();
        }
        
        public static T Parse<T>(string fileName)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(ConfigurationTypeEnum.PreferEnvironment, fileName)));
            return parser.ParseConfiguration<T>();
        }

        public static T Parse<T>(string fileName, ConfigurationTypeEnum configurationTypeEnum)
        {
            var parser = new ConfigurationParser(new EnvironmentVariableRepository(
                new EnvironmentVariableSource(), 
                new FileVariableSource(configurationTypeEnum, fileName),
                configurationTypeEnum));
            return parser.ParseConfiguration<T>();
        }
        
        public static T Parse<T>(IEnvironmentVariableRepository env)
        {
            var parser = new ConfigurationParser(env);
            return parser.ParseConfiguration<T>();
        }
        
        public T ParseConfiguration<T>()
        {
            var type = typeof(T);
            var instance = (T)FormatterServices.GetUninitializedObject(type);
            GetProperties(instance, type, null, new Stack<Type>());

            return instance;
        }
        
        private void GetProperties<T>(T instance, Type type, string prefix, Stack<Type> recursive)
        {
            Push(recursive, type);
            
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.MemberType != MemberTypes.Property)
                {
                    continue;
                }

                var itemName = prop.Name;
                var required = true;
                object @default = null;
                
                var configItemAttr = prop.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(ConfigItem));
                var ignoreConfigItemAttr = prop.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(IgnoreConfigItem));
                if (ignoreConfigItemAttr is IgnoreConfigItem)
                {
                    continue;
                }
                
                if (configItemAttr is ConfigItem cAttr)
                {
                    itemName = cAttr.Name;
                    required = cAttr.Required;
                    @default = cAttr.Default;
                }

                var isNullable = Nullable.GetUnderlyingType(prop.PropertyType);
                
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || isNullable != null)
                {
                    var val = _environmentVariableRepository.GetEnvironmentVariable(prefix != null ? $"{prefix + "_" ?? ""}{itemName}" : itemName);
                    if (required && string.IsNullOrEmpty(val))
                    {
                        throw new KeyNotFoundException(
                            $"[CustomEnvironmentConfig] Required configuration environment variable is empty: " +
                            $"{type.Name}.{prop.Name} (looking for env '{(prefix != null ? $"{prefix + "_" ?? ""}{itemName}" : itemName)}'.");
                    }

                    if (!required && string.IsNullOrEmpty(val))
                    {
                        if (@default != null)
                        {
                            var conv = Convert.ChangeType(@default, (isNullable ?? prop.PropertyType));
                            prop.SetValue(instance, conv);
                        }
                        continue;
                    }

                    // try to cast env to that type.
                    var convertedVal = Convert.ChangeType(val, (isNullable ?? prop.PropertyType));

                    // set property in instance.
                    prop.SetValue(instance, convertedVal);
                }
                else
                {
                    // Create new instance of type.
                    var subInstance = FormatterServices.GetUninitializedObject(prop.PropertyType);
                    GetProperties(subInstance, prop.PropertyType, $"{(prefix != null ? prefix + "_" : "")}{itemName}", recursive);
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
    }
}