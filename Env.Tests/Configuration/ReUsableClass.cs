namespace Env.Tests.Configuration
{
    public class ReUsableClass
    {
        [ConfigItem]
        public ReUsableSubClass SubClass1 { get; set; }
        
        [ConfigItem]
        public ReUsableSubClass SubClass2 { get; set; }
        
        [ConfigItem]
        public ReUsableSubClass SubClass3 { get; set; }
    }
}