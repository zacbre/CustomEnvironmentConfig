namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class UnderscoreClass
    {
        [ConfigurationItem("SUB_CLASS")]
        public required UnderscoreSubClass SubClass { get; set; }
    }

    public class UnderscoreSubClass
    {
        [ConfigurationItem("MY_ITEM")]
        public required string MyItem { get; set; }
    }
}