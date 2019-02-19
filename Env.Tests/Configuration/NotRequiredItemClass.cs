namespace Env.Tests.Configuration
{
    public class NotRequiredItemClass
    {
        [ConfigItem]
        public string RequiredItem { get; set; }
        
        [ConfigItem(Required = ConfigItemRequirement.NotRequired)]
        public string NotRequiredItem { get; set; }
    }
}