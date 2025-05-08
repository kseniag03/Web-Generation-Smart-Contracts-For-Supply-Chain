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
                var dirInfo2 = new DirectoryInfo(fromConfig);

                _cachedPath = GetSolutionPath(dirInfo2);
            }

            var dirInfo1 = new DirectoryInfo(Directory.GetCurrentDirectory());

            _cachedPath = 
                (
                    _cachedPath is null 
                    ? GetSolutionPath(dirInfo1) 
                    : _cachedPath
                )
                ?? throw new InvalidOperationException("Solution directory not found.");

            return _cachedPath;
        }

        private static string? GetSolutionPath(DirectoryInfo dirInfo)
        {
            if (dirInfo is null || !dirInfo.Exists)
            {
                return null;
            }

            while (dirInfo is not null && !dirInfo.GetFiles("*.sln", SearchOption.AllDirectories).Any() && dirInfo?.Parent is not null)
            {
                dirInfo = dirInfo.Parent;
            }

            _cachedPath = dirInfo?.FullName ?? throw new InvalidOperationException("Solution directory not found.");

            return _cachedPath;
        }
    }
}
