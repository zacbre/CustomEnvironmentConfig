namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class NotRequiredItemClass
    {
        public string? RequiredItem { get; set; }
        
        [ConfigurationItem(Required = false)]
        public string? NotRequiredItem { get; set; }
    }
}