using System.Text.Json;
using Application.Interfaces;
using Core.UseCases;
using Infrastructure.Repositories.Helpers;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class SmartContractRepository : ISmartContractRepository
    {
        private readonly string _solutionDirectory;
        private readonly string _configsPath;

        public SmartContractRepository(IConfiguration configuration)
        {
            var configsPath = configuration["HardhatConfigsPath"];

            if (string.IsNullOrEmpty(configsPath))
            {
                throw new ArgumentException("Configs path is not found.");
            }

            _solutionDirectory = SolutionPathHelper.GetSolutionRoot(configuration);
            _configsPath = Path.Combine(_solutionDirectory, configsPath);
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

        public void CopyBaseHardhatConfigs(string instancePath)
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
