namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class DefaultValueClass
    {
        [ConfigurationItem(Default = true, Required = false)]
        public bool HasDefault { get; set; }

        [ConfigurationItem(Required = false)] 
        public bool DoesNotHaveDefault { get; set; } = true;
    }
    
    public class DefaultValueWeirdTypesClass
    {
        [ConfigurationItem(Default = 1, Required = false)]
        public bool HasDefault { get; set; }
        
        [ConfigurationItem(Default = "true", Required = false)]
        public bool DoesNotHaveDefault { get; set; }
    }

    public class DefaultValueStringClass
    {
        [ConfigurationItem(Default = "i have a value!", Required = false)]
        public string? DoIHaveAValue { get; set; }
    }
    
    public class DefaultValueBrokenClass
    {
        [ConfigurationItem(Default = "y", Required = false)]
        public int HasDefault { get; set; }
        
        [ConfigurationItem(Default = -23451, Required = false)]
        public bool DoesNotHaveDefault { get; set; }
    }

    public class AllDefaultTestsClass
    {
        [ConfigurationItem(Default = 4, Required = false)]
        public int IntDefault { get; set; }
    }
}