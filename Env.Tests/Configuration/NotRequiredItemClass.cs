namespace Env.Tests.Configuration
{
    public class NotRequiredItemClass
    {
        public string RequiredItem { get; set; }
        
        [ConfigItem(Required = false)]
        public string NotRequiredItem { get; set; }
    }
}