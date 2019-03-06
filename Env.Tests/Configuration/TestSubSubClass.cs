namespace Env.Tests.Configuration
{
    public class TestSubSubClass
    {
        [ConfigItem]
        public string Item { get; set; }
         
        [ConfigItem]
        public bool Bool { get; set; }
        
        [ConfigItem]
        public int Int { get; set; }
        
        [ConfigItem]
        public long Long { get; set; }
        
        [ConfigItem]
        public double Double { get; set; }
        
        [ConfigItem]
        public float Float { get; set; }
    }
}