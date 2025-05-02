using System.Text.Json;
using System.Text.RegularExpressions;
using Core.Interfaces;
using Core.UseCases;
using Infrastructure.Repositories.Helpers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class SmartContractRepository : ISmartContractRepository
    {
        private readonly string _solutionDirectory;
        private readonly string _templatesPath;
        private readonly string _configsPath;

        public SmartContractRepository(IConfiguration configuration)
        {
            var tempatesPath = configuration["ContractTemplatesPath"];
            var configsPath = configuration["HardhatConfigsPath"];

            if (string.IsNullOrEmpty(tempatesPath) || string.IsNullOrEmpty(configsPath))
            {
                throw new ArgumentException("Templates or configs path is not found.");
            }

            _solutionDirectory = SolutionPathHelper.GetSolutionRoot(configuration);
            _templatesPath = Path.Combine(_solutionDirectory, tempatesPath);
            _configsPath = Path.Combine(_solutionDirectory, configsPath);
        }

        public string GenerateContractCode(string contractName, string appArea, string uintType, bool enableEvents, bool includeVoid, string instancePath)
        {
            var filePath = Path.Combine(_templatesPath, "template-contract.sol");

            if (!File.Exists(filePath))
            {
                return $"Error: Contract template for '{appArea}' not found in {filePath}";
            }

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
                enableEvents ? "$1" : "",
                RegexOptions.Multiline);

            processedContract = Regex.Replace(processedContract,
                @"\{VOID_OPTIONAL\}([\s\S]*?)\{VOID_OPTIONAL\}",
                includeVoid ? "$1" : "",
                RegexOptions.Multiline);

            var contractPath = Path.Combine(instancePath, "contracts");

            Directory.CreateDirectory(contractPath);

            var contractFilePath = Path.Combine(contractPath, $"{contractName}.sol");

            File.WriteAllText(contractFilePath, processedContract);

            GenerateTestScript(contractName, enableEvents, includeVoid, instancePath);
            CopyBaseHardhatConfigs(instancePath);

            return processedContract;
        }

        public string GetContractCode(string contractName, string instancePath) => $"contact {contractName} code: WANTED";

        public string GetDeployedContractAddress(string contractName, string instancePath) => $"address of deployed contact {contractName}: WANTED";

        public DeploySmartContractEntities? GetContractAbiBytecode(string contractName, string instancePath)
        {
            var path = Path.Combine(instancePath, "artifacts", "contracts", $"{contractName}.sol", $"{contractName}.json");

            if (!File.Exists(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);
            var parsed = JsonDocument.Parse(json).RootElement;

            return new DeploySmartContractEntities
            {
                Abi = parsed.GetProperty("abi"),
                Bytecode = parsed.GetProperty("bytecode").GetString()?.TrimStart('0', 'x')
            };
        }

        private void GenerateTestScript(string contractName, bool enableEvents, bool includeVoid, string instancePath)
        {
            var filePath = Path.Combine(_templatesPath, "template-test.js");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: Test template not found.");

                return;
            }

            var template = File.ReadAllText(filePath);
            var processedScript = Regex.Replace(template, @"\{CONTRACT_NAME\}", contractName);

            // remove or keep the content between the tags
            processedScript = Regex.Replace(processedScript,
                @"\{EVENTS_OPTIONAL\}([\s\S]*?)\{EVENTS_OPTIONAL\}",
                enableEvents ? "$1" : "",
                RegexOptions.Multiline);

            processedScript = Regex.Replace(processedScript,
                @"\{VOID_OPTIONAL\}([\s\S]*?)\{VOID_OPTIONAL\}",
                includeVoid ? "$1" : "",
                RegexOptions.Multiline);

            var testPath = Path.Combine(instancePath, "test");

            Directory.CreateDirectory(testPath);

            var scriptFilePath = Path.Combine(testPath, $"test_{contractName}.js");

            File.WriteAllText(scriptFilePath, processedScript);
        }

        private void CopyBaseHardhatConfigs(string instancePath)
        {
            CopyIfMissing("hardhat.config.ts", instancePath);
            CopyIfMissing("tsconfig.json", instancePath);
            CopyIfMissing("package.json", instancePath);
            CopyIfMissing("package-lock.json", instancePath); // if using fixed dependencies
        }

        private void CopyIfMissing(string fileName, string instancePath)
        {
            var source = Path.Combine(_configsPath, fileName);
            var dest = Path.Combine(instancePath, fileName);

            if (!File.Exists(dest))
            {
                File.Copy(source, dest);
            }
        }

    }
}
