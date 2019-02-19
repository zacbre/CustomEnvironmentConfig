namespace Env.Tests.Configuration
{
    public class TestClass
    {
        [ConfigItem]
        public TestSubClass SubItem { get; set; }
        
        [ConfigItem]
        public string Item { get; set; }
    }
}