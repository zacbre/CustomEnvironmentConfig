using System;
using System.Collections.Generic;

namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class ConvertibleClass
    {
        [ConfigurationItem(Json = true)]
        public List<QueueItems> QueueItems { get; set; }
        
        [ConfigurationItem("Custom_Class", Json=true)]
        public CustomClass CustomClass { get; set; }
    }

    public class QueueItems
    {
        public string Url { get; set; }
        public bool Enabled { get; set; }
    }

    public class CustomClass
    {
        public string Test { get; set; }
        public int Number { get; set; }
    }
}