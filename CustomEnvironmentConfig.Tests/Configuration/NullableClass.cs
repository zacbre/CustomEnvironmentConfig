namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class NullableClass
    {
        public int? Test { get; set; }
        public bool? TestBool { get; set; }
        
        [ConfigurationItem(Required = false)]
        public int? NotRequired { get; set; }
    }
}