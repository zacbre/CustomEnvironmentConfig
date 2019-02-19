namespace Env.Interfaces
{
    public interface IEnvironmentVariableRepository
    {
        string GetEnvironmentVariable(string keyName);
    }
}