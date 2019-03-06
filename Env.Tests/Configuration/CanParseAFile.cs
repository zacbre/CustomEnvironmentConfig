namespace Env.Tests.Configuration
{
    public class CanParseAFile
    {
        [ConfigItem]
        public string Test { get; set;}
        
        [ConfigItem]
        public int Test1 { get; set; }
        
        [ConfigItem]
        public bool Test2 { get; set; }
    }
}