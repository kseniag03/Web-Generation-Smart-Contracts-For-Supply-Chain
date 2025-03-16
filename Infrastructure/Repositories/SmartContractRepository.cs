using System.Text.RegularExpressions;
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
            var filePath = Path.Combine(_templatesPath, $"template-test.sol");

            if (!File.Exists(filePath))
                return $"Error: Contract template for '{contractName}' not found";

            string uintType = "uint8";
            bool includeEvents = false;
            bool includeVoid = false;

            var template = File.ReadAllText(filePath);
            var processedContract = template;

            var replacements = new Dictionary<string, string>
            {
                { @"\{CONTRACT_NAME\}", contractName },
                { @"\{UINT_TYPE\}", uintType }
            };

            foreach (var kvp in replacements)
            {
                processedContract = Regex.Replace(processedContract, kvp.Key, kvp.Value);
            }

            // remove or keep the content between the tags
            processedContract = Regex.Replace(processedContract,
                @"\{EVENTS_OPTIONAL\}([\s\S]*?)\{EVENTS_OPTIONAL\}",
                includeEvents ? "$1" : "",
                RegexOptions.Multiline);

            processedContract = Regex.Replace(processedContract,
                @"\{VOID_OPTIONAL\}([\s\S]*?)\{VOID_OPTIONAL\}",
                includeVoid ? "$1" : "",
                RegexOptions.Multiline);

            return processedContract;
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
