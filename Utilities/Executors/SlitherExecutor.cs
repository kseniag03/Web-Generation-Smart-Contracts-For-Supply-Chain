using Microsoft.Extensions.Configuration;
using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class SlitherExecutor : ISlitherExecutor
    {
        private readonly string _baseDir = AppContext.BaseDirectory;
        private readonly ICommandExecutor _cmd;

        public SlitherExecutor(IConfiguration config, ICommandExecutor cmd)
        {
            _cmd = cmd;
        }

        public Task<string> SetupInstanceEnvironment(string instancePath)
        {
            throw new NotImplementedException();
        }

        public Task<string> RunAnalysis(string instancePath)
        {
            var script = GetAbsolutePath("Utilities/run-sither.sh");

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
