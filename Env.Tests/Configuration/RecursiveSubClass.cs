namespace Env.Tests.Configuration
{
    public class RecursiveSubClass
    {
        [ConfigItem]
        public RecursiveClass RecursiveClass { get; set; }
        
        [ConfigItem]
        public string Item { get; set; }
    }
}