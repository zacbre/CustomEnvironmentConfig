namespace Env.Tests.Configuration
{
    public class NullableClass
    {
        public int? Test { get; set; }
        public bool? TestBool { get; set; }
        
        [ConfigItem(Required = ConfigItemRequirement.NotRequired)]
        public int? NotRequired { get; set; }
    }
}