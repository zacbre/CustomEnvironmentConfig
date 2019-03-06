namespace Env.Tests.Configuration
{
    public class NotRequiredItemClass
    {
        public string RequiredItem { get; set; }
        
        [ConfigItem(Required = ConfigItemRequirement.NotRequired)]
        public string NotRequiredItem { get; set; }
    }
}