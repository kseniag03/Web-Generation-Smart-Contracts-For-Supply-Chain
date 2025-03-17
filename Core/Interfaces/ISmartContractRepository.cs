namespace Core.Interfaces
{
    public interface ISmartContractRepository
    {
        string GenerateContractCode(string contractName, string appArea, string uintType, bool enableEvents, bool includeVoid);
        string GetContractCode(string contractName);
        string GetDeployedContractAddress(string contractName);
    }
}
