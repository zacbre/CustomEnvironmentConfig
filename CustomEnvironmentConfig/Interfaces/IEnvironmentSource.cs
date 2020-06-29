namespace CustomEnvironmentConfig.Interfaces
{
    public interface IEnvironmentSource
    {
        string? Get(string keyName);
    }
}