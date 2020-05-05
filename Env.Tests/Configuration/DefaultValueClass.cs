namespace Env.Tests.Configuration
{
    public class DefaultValueClass
    {
        [ConfigItem(Default = true, Required = false)]
        public bool HasDefault { get; set; }

        [ConfigItem(Required = false)] 
        public bool DoesNotHaveDefault { get; set; } = true;
    }
    
    public class DefaultValueWeirdTypesClass
    {
        [ConfigItem(Default = 1, Required = false)]
        public bool HasDefault { get; set; }
        
        [ConfigItem(Default = "true", Required = false)]
        public bool DoesNotHaveDefault { get; set; }
    }
    
    public class DefaultValueBrokenClass
    {
        [ConfigItem(Default = "y", Required = false)]
        public int HasDefault { get; set; }
        
        [ConfigItem(Default = -23451, Required = false)]
        public bool DoesNotHaveDefault { get; set; }
    }
}