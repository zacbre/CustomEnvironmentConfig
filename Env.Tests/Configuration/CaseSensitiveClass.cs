namespace Env.Tests.Configuration
{
    public class CaseSensitiveClass
    {
        [ConfigItem]
        public string Test { get; set; }
        
        [ConfigItem]
        public string test { get; set; }
    }
}