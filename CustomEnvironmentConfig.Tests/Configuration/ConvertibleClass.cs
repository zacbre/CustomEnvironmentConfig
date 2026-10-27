using System.Collections.Generic;

namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class ConvertibleClass
    {
        [ConfigurationItem(Json = true)]
        public required List<QueueItems> QueueItems { get; set; }
        
        [ConfigurationItem("Custom_Class", Json=true)]
        public required CustomClass CustomClass { get; set; }
        
        [ConfigurationItem("Custom_Class1")]
        public required string CustomClass1 { get; set; }
    }

    public class QueueItems
    {
        public required string Url { get; set; }
        public bool Enabled { get; set; }
    }

    public class CustomClass
    {
        public required string Test { get; set; }
        public int Number { get; set; }
    }
}