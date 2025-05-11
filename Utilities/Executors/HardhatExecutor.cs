using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class HardhatExecutor : IHardhatExecutor
    {
        private readonly ICommandExecutor _cmd;
        private readonly string _baseDir = AppContext.BaseDirectory;
        private readonly string _scriptPath;
        private readonly string _scriptCheckPath;

        public HardhatExecutor(ICommandExecutor cmd)
        {
            _cmd = cmd;
            _scriptPath = Path.Combine(_baseDir, "Utilities/run-hardhat.sh");
            _scriptCheckPath = Path.Combine(_baseDir, "Utilities/check-hardhat.sh");
        }

        public async Task<string> SetupInstanceEnvironment(string instancePath)
        {
            var absPath = instancePath.StartsWith("/app")
                ? instancePath
                : Path.Combine(_baseDir, instancePath.Replace('\\', '/'));

            DirectoryInfo dir = new(absPath);

            if (!dir.Exists)
            {
                Console.WriteLine($"Directory not found: {absPath}");

                throw new Exception($"Directory not found: {absPath}");
            }

            var instanceId = new FileInfo(instancePath).Name;

            return await _cmd.ExecuteCommandAsync(
                "/bin/bash",
                $"{_scriptCheckPath} {instanceId}",
                _baseDir
            );

        }

        public async Task<string> CompileContract(string instancePath)
        {
            var output = await RunScript("compile", instancePath);

            Console.WriteLine("=== Hardhat Compile Output ===");
            Console.WriteLine(output);
            Console.WriteLine("=== Hardhat Compile Output Finish ===");

            return output;
        }

        public async Task<string> TestContract(string instancePath)
        {
            var output = await RunScript("test", instancePath);

            Console.WriteLine("=== Hardhat Test Output ===");
            Console.WriteLine(output);
            Console.WriteLine("=== Hardhat Test Output Finish ===");

            return output;
        }

        private async Task<string> RunScript(string command, string instancePath)
        {
            foreach (var file in Directory.EnumerateFiles(instancePath))
            {
                Console.WriteLine("!!! File: " + file);
            }

            var instanceId = new FileInfo(instancePath).Name;

            return await _cmd.ExecuteCommandAsync(
                "/bin/bash",
                $"{_scriptPath} {command} {instanceId}",
                _baseDir
            );
        }
    }
}
