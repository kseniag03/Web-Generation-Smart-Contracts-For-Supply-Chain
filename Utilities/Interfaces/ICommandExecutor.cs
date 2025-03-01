namespace Utilities.Interfaces
{
    public interface ICommandExecutor
    {
        Task<string> ExecuteCommandAsync(string command, string args);
    }
}
