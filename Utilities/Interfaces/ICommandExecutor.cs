namespace Utilities.Interfaces
{
    public interface ICommandExecutor
    {
        Task<string> ExecuteCommandAsync(string fileName, string arguments, string? workingDirectory = null);
    }
}
