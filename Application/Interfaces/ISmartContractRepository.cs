using Core.UseCases;

namespace Core.Interfaces
{
    public interface ISmartContractRepository
    {
        string GenerateContractCode(string contractName, string appArea, string uintType, bool enableEvents, bool includeVoid, string instancePath);
        string GetContractCode(string contractName, string instancePath);
        string GetDeployedContractAddress(string contractName, string instancePath);
        DeploySmartContractEntities? GetContractAbiBytecode(string contractName, string instancePath);
    }
}
