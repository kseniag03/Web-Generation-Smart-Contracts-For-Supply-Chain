using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Repositories.Helpers
{
    public class SolutionPathProvider : ISolutionPathProvider
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _hostEnv;

        public SolutionPathProvider(IConfiguration config, IHostEnvironment hostEnv)
        {
            _config = config;
            _hostEnv = hostEnv;
        }

        public string GetSolutionRoot() => SolutionPathHelper.GetSolutionRoot(_config, _hostEnv);
    }
}
