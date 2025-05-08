using Core.UseCases;

namespace Application.Interfaces
{
    public interface ISmartContractRepository
    {
        string GetContractCode(string contractName, string instancePath);
        string GetDeployedContractAddress(string contractName, string instancePath);
        DeploySmartContractEntities? GetContractAbiBytecode(string contractName, string instancePath);
        void CopyBaseHardhatConfigs(string instancePath);
    }
}
