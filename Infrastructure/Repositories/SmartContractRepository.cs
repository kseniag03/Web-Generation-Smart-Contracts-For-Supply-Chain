using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class SmartContractRepository : ISmartContractRepository
    {
        private static readonly Lazy<string> _solutionDirectory = new(() => FindSolutionDirectory());
        private readonly string _templatesPath;

        public SmartContractRepository(IConfiguration configuration)
        {
            var tempatepath = configuration["ContractTemplatesPath"];

            if (string.IsNullOrEmpty(_solutionDirectory.Value) || string.IsNullOrEmpty(tempatepath))
            {
                throw new ArgumentException("Solution directory or template path is not found.");
            }

            _templatesPath = Path.Combine(_solutionDirectory.Value, tempatepath);
        }

        public string GenerateContractCode(string contractName)
        {
            var filePath = Path.Combine(_templatesPath, $"{contractName}.txt");

            if (!File.Exists(filePath))
                return $"Error: Contract template '{contractName}' not found";

            return File.ReadAllText(filePath);
        }

        public string GetContractCode(string contractName) => $"contact {contractName} code: WANTED";

        public string GetDeployedContractAddress(string contractName) => $"address of deployed contact {contractName}: WANTED";

        private static string FindSolutionDirectory()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (dir is not null && !dir.GetFiles("*.sln").Any())
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? throw new InvalidOperationException("Solution directory not found.");
        }
    }
}
