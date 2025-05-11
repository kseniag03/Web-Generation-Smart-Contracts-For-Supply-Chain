using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Repositories.Helpers
{
    public static class SolutionPathHelper
    {
        private static string? _cachedPath;

        public static string GetSolutionRoot(IConfiguration configuration, IHostEnvironment hostEnv)
        {
            if (!string.IsNullOrEmpty(_cachedPath))
            {
                return _cachedPath;
            }

            var cfg = configuration["SolutionRoot"];

            if (!string.IsNullOrWhiteSpace(cfg))
            {
                var full = Path.GetFullPath(cfg);

                if (Directory.Exists(full))
                {
                    _cachedPath = full;

                    return _cachedPath;
                }
            }

            var dir = new DirectoryInfo(hostEnv.ContentRootPath);

            while (dir != null)
            {
                if (dir.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Any())
                {
                    _cachedPath = dir.FullName;

                    return _cachedPath;
                }

                dir = dir.Parent;
            }

            throw new InvalidOperationException("Solution directory not found");
        }
    }
}
