namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class UnderscoreClass
    {
        [ConfigurationItem("SUB_CLASS")]
        public UnderscoreSubClass SubClass { get; set; }
    }

    public class UnderscoreSubClass
    {
        [ConfigurationItem("MY_ITEM")]
        public string MyItem { get; set; }
    }
}