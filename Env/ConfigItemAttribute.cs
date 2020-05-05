using System;
using System.Runtime.CompilerServices;

namespace Env
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreConfigItem : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigItem : Attribute
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public object Default { get; set; }
        
        public ConfigItem([CallerMemberName] string propertyName = "ENV", bool required = true, object @default = null)
        {
            Name = propertyName;
            Required = required;
            Default = @default;
        }
    }
}