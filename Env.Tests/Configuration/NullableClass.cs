namespace Env.Tests.Configuration
{
    public class NullableClass
    {
        public int? Test { get; set; }
        public bool? TestBool { get; set; }
        
        [ConfigItem(Required = false)]
        public int? NotRequired { get; set; }
    }
}