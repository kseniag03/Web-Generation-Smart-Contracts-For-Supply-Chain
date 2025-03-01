using Utilities.Helpers;
using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class WindowsCommandExecutor : ICommandExecutor
    {
        public async Task<string> ExecuteCommandAsync(string command, string args)
        {
            return await CommandRunner.RunCommandAsync("cmd.exe", $"/C {command} {args}");
        }
    }

}
