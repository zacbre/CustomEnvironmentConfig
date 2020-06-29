namespace CustomEnvironmentConfig.Interfaces
{
    public interface IEnvironmentVariableRepository
    {
        string? GetEnvironmentVariable(string keyName);
    }
}