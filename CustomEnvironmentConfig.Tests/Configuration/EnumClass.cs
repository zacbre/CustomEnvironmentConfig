namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class EnumClass
    {
        public TestEnum Enum { get; set; }
        
        [ConfigurationItem(Required = false, Default = TestEnum.Item1)]
        public TestEnum NotRequired { get; set; }
    }

    public enum TestEnum
    {
        Item1,
        Item2
    }
}