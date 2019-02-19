using System;
using System.Runtime.CompilerServices;

namespace Env
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigItem : Attribute
    {
        public string Name { get; set; }
        public ConfigItemRequirement Required { get; set; }
        
        public ConfigItem([CallerMemberName] string propertyName = "ENV", ConfigItemRequirement requirement = ConfigItemRequirement.Required)
        {
            Name = propertyName;
            Required = requirement;
        }
    }

    public enum ConfigItemRequirement
    {
        Required,
        NotRequired
    }
}