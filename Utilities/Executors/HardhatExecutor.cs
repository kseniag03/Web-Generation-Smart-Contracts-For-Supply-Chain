using Microsoft.Extensions.Configuration;
using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class HardhatExecutor : IHardhatExecutor
    {
        private readonly string _baseDir = AppContext.BaseDirectory;
        private readonly ICommandExecutor _cmd;

        public HardhatExecutor(IConfiguration config, ICommandExecutor cmd)
        {
            _cmd = cmd;
        }

        public Task<string> CompileContract(string instancePath)
        {
            var script = GetAbsolutePath("Utilities/run-hardhat.sh");

            return _cmd.ExecuteCommandAsync(
                "/bin/bash",
                $"{script} compile {instancePath}",
                _baseDir
            );
        }

        public Task<string> TestContract(string instancePath)
        {
            var script = GetAbsolutePath("Utilities/run-hardhat.sh");

            return _cmd.ExecuteCommandAsync(
                "/bin/bash",
                $"{script} test {instancePath}",
                _baseDir
            );
        }

        private string GetAbsolutePath(string relativePath) =>
            Path.Combine(_baseDir, relativePath);
    }
}
