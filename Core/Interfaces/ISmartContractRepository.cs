namespace Core.Interfaces
{
    public interface ISmartContractRepository
    {
        string GenerateContractCode(string name);
        string GetContractCode(string contractName);
        string GetDeployedContractAddress(string contractName);
    }
}
