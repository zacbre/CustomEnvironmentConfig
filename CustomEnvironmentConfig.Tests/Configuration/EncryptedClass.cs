namespace CustomEnvironmentConfig.Tests.Configuration
{
    public class EncryptedClass
    {
        [ConfigurationItem(Encrypt = true)]
        public string? Item1 { get; set; }
        
        [ConfigurationItem(Encrypt = true)]
        public int Item2 { get; set; }
        
        [ConfigurationItem(Encrypt = true)]
        public bool Item3 { get; set; }
    }
}