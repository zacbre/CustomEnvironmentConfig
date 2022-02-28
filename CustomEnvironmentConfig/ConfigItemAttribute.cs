using System;
using System.Runtime.CompilerServices;

namespace CustomEnvironmentConfig
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationItem : Attribute
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool Ignore { get; set; }
        public object? Default { get; set; }
        public bool Encrypt { get; set; }
        public bool Json { get; set; }
        
        /// <summary>
        /// Specifies where your configuration should come from.
        /// </summary>
        public ConfigurationTypeEnum ConfigurationType { get; set; }
        
        public ConfigurationItem([CallerMemberName] string propertyName = "ENV", bool required = true, bool ignore = false, bool encrypt = false, object? @default = null, ConfigurationTypeEnum configurationType = ConfigurationTypeEnum.Default, bool json = false)
        {
            Name = propertyName;
            Required = required;
            Ignore = ignore;
            Encrypt = encrypt;
            Default = @default;
            ConfigurationType = configurationType;
            Json = json;
        }
    }
}