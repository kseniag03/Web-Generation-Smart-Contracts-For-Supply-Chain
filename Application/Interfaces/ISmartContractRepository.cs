using Application.DTOs;

namespace Application.Interfaces
{
    public interface ISmartContractRepository
    {
        string GetContractCode(string contractName, string instancePath);
        string GetDeployedContractAddress(string contractName, string instancePath);
        AbiBytecodeDto? GetContractAbiBytecode(string contractName, string instancePath);
        void CopyBaseHardhatConfigs(string instancePath);
    }
}
