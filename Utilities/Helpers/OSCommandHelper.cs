using System.Runtime.InteropServices;

namespace Utilities.Helpers
{
    public static class OSCommandHelper
    {
        public static string GetShell()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "cmd.exe";
            return "/bin/bash";
        }

        public static string GetCommandPrefix()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "/C"; // Для cmd
            return "-c";   // Для bash
        }
    }
}
