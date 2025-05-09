using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories.Helpers
{
    public static class SolutionPathHelper
    {
        private static string? _cachedPath;

        public static string GetSolutionRoot(IConfiguration configuration)
        {
            if (!string.IsNullOrEmpty(_cachedPath))
            {
                return _cachedPath;
            }

            DirectoryInfo? dirInfo = null;

            var fromConfig = configuration["SolutionRoot"];

            if (!string.IsNullOrEmpty(fromConfig))
            {
                var configDir = new DirectoryInfo(fromConfig);

                if (configDir.Exists && configDir.GetFiles("*.sln", SearchOption.AllDirectories).Any())
                {
                    dirInfo = configDir;
                }
            }

            if (dirInfo == null)
            {
                dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            var solutionPath = GetSolutionPath(dirInfo)
                ?? throw new InvalidOperationException("Solution directory not found.");

            _cachedPath = solutionPath;

            return _cachedPath;
        }

        private static string? GetSolutionPath(DirectoryInfo dirInfo)
        {
            while (dirInfo != null && dirInfo.Parent != null)
            {
                if (dirInfo.GetFiles("*.sln", SearchOption.AllDirectories).Any())
                {
                    return dirInfo.FullName;
                }

                dirInfo = dirInfo.Parent;
            }

            return null;
        }
    }
}
