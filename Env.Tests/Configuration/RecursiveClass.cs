namespace Env.Tests.Configuration
{
    public class RecursiveClass
    {
        [ConfigItem]
        public RecursiveSubClass RecursiveSubClass { get; set; }
    }
}