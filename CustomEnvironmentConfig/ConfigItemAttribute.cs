using System;
using System.Runtime.CompilerServices;

namespace CustomEnvironmentConfig
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationItem(
        [CallerMemberName] string propertyName = "ENV",
        bool required = true,
        bool ignore = false,
        bool encrypt = false,
        object? @default = null,
        ConfigurationTypeEnum configurationType = ConfigurationTypeEnum.Default,
        bool json = false)
        : Attribute
    {
        public string Name { get; set; } = propertyName;
        public bool Required { get; set; } = required;
        public bool Ignore { get; set; } = ignore;
        public object? Default { get; set; } = @default;
        public bool Encrypt { get; set; } = encrypt;
        public bool Json { get; set; } = json;

        /// <summary>
        /// Specifies where your configuration should come from.
        /// </summary>
        public ConfigurationTypeEnum ConfigurationType { get; set; } = configurationType;
    }
}