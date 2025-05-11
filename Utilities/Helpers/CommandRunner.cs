using System.Diagnostics;

namespace Utilities.Helpers
{
    public class CommandRunner
    {
        public static async Task<string> RunCommandAsync(string command, string args)
        {
            return await RunCommandAsync(command, args, null);
        }

        public static async Task<string> RunCommandAsync(string command, string args, string? workingDirectory)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            var stdOutTask = process.StandardOutput.ReadToEndAsync();
            var stdErrTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = await stdOutTask;
            var error = await stdErrTask;

            Console.WriteLine("=== COMMAND EXECUTION ===");
            Console.WriteLine($"Command: {command} {args}");
            Console.WriteLine($"WorkingDir: {workingDirectory}");
            Console.WriteLine("Output:");
            Console.WriteLine(output);
            Console.WriteLine("Error:");
            Console.WriteLine(error);
            Console.WriteLine("=== COMMAND EXECUTION FINISH ===");

            return string.IsNullOrWhiteSpace(error) ? output : output + "\n--- STDERR ---\n" + error;
        }
    }
}
