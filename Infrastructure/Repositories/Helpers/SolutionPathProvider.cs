using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories.Helpers
{
    public class SolutionPathProvider : ISolutionPathProvider
    {
        private readonly IConfiguration _config;

        public SolutionPathProvider(IConfiguration config) => _config = config;

        public string GetSolutionRoot() => SolutionPathHelper.GetSolutionRoot(_config);
    }
}
