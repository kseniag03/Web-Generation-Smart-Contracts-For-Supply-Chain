using Utilities.Helpers;
using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class CommandExecutor : ICommandExecutor
    {
        public Task<string> ExecuteCommandAsync(string fileName, string arguments, string? workingDirectory = null)
        {
            return CommandRunner.RunCommandAsync(fileName, arguments, workingDirectory);
        }
    }
}
