using Utilities.Helpers;
using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class UnixCommandExecutor : ICommandExecutor
    {
        public async Task<string> ExecuteCommandAsync(string command, string args)
        {
            return await CommandRunner.RunCommandAsync("/bin/bash", $"-c \"{command} {args}\"");
        }
    }

}
