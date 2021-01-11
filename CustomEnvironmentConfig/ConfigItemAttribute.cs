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
        
        public ConfigurationItem([CallerMemberName] string propertyName = "ENV", bool required = true, bool ignore = false, bool encrypt = false, object? @default = null)
        {
            Name = propertyName;
            Required = required;
            Ignore = ignore;
            Encrypt = encrypt;
            Default = @default;
        }
    }
}