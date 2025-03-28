﻿using System.Diagnostics;

namespace Utilities.Helpers
{
    public class CommandRunner
    {
        public static async Task<string> RunCommandAsync(string command, string args)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                return string.IsNullOrEmpty(error) ? output : error;
            }
        }
    }
}
