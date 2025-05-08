using Microsoft.Extensions.Configuration;
using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class FoundryExecutor : IFoundryExecutor
    {
        private readonly string _baseDir = AppContext.BaseDirectory;
        private readonly ICommandExecutor _cmd;

        public FoundryExecutor(IConfiguration config, ICommandExecutor cmd)
        {
            _cmd = cmd;
        }

        public Task<string> SetupInstanceEnvironment(string instancePath)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetGasReport(string instancePath, string args = "")
        {
            var script = GetAbsolutePath("Utilities/run-foundry.sh");

            return _cmd.ExecuteCommandAsync(
                "/bin/bash",
                $"{script} {instancePath}",
                _baseDir
            );
        }

        private string GetAbsolutePath(string relativePath) =>
            Path.Combine(_baseDir, relativePath);
    }
}
