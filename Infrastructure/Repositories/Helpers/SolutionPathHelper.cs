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

            var fromConfig = configuration["SolutionRoot"];

            if (!string.IsNullOrEmpty(fromConfig))
            {
                _cachedPath = fromConfig;
                return _cachedPath;
            }

            // fallback only for local dev
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (dir is not null && !dir.GetFiles("*.sln").Any())
            {
                dir = dir.Parent;
            }

            _cachedPath = dir?.FullName ?? throw new InvalidOperationException("Solution directory not found.");

            return _cachedPath;
        }
    }
}
