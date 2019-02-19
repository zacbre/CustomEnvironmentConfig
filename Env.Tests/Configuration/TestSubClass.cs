namespace Env.Tests.Configuration
{
    public class TestSubClass
    {
        [ConfigItem]
        public TestSubSubClass SubSubItem { get; set; }
        
        [ConfigItem]
        public string Item { get; set; }
    }
}