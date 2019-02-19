using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Env.Interfaces;

namespace Env
{
    public class ConfigurationParser
    {
        private readonly IEnvironmentVariableRepository _environmentVariableRepository;
        private List<Type> _recursivePrevention;
        
        public ConfigurationParser(IEnvironmentVariableRepository environmentVariableRepository)
        {
            _environmentVariableRepository = environmentVariableRepository;

            _recursivePrevention = new List<Type>();
        }
        public T ParseConfiguration<T>()
        {
            var type = typeof(T);
            var instance = (T)Activator.CreateInstance(type);
            GetProperties(instance, type, null);

            return instance;
        }
        
        private void GetProperties<T>(T instance, Type type, string prefix)
        {
            if (_recursivePrevention.Contains(type))
            {
                throw new StackOverflowException($"Eternal class creation loop... Type already initialized: {type.ToString()}");
            }
            _recursivePrevention.Add(type);
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.MemberType != MemberTypes.Property)
                {
                    continue;
                }

                // Make sure it has no attributes that match our "ConfigItem" attribute.
                var attr = prop.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(ConfigItem));
                if (attr is ConfigItem cAttr)
                {
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                    {
                        var val = _environmentVariableRepository.GetEnvironmentVariable(prefix != null ? $"{prefix + "_" ?? ""}{cAttr.Name}" : cAttr.Name);
                        if (cAttr.Required == ConfigItemRequirement.Required && string.IsNullOrEmpty(val))
                        {
                            throw new Exception(
                                $"[CustomEnvironmentConfig] Required configuration environment variable is empty: " +
                                $"{type.Name}.{prop.Name} (looking for env '{(prefix != null ? $"{prefix + "_" ?? ""}{cAttr.Name}" : cAttr.Name)}'.");
                        }

                        // try to cast env to that type.
                        var convertedVal = Convert.ChangeType(val, prop.PropertyType);

                        // set property in instance.
                        prop.SetValue(instance, convertedVal);
                    }
                    else
                    {
                        // Create new instance of type.
                        var subInstance = Activator.CreateInstance(prop.PropertyType);
                        GetProperties(subInstance, prop.PropertyType, $"{(prefix != null ? prefix + "_" : "")}{cAttr.Name}");
                        prop.SetValue(instance, subInstance);
                    }
                }
            }
        }
    }
}