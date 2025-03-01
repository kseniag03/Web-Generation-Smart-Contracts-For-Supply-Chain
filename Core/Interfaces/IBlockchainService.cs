namespace Core.Interfaces
{
    public interface IBlockchainService
    {
        Task<string> DeployContractAsync(string contractCode);
        Task<bool> TestContractAsync(string contractName);
        Task<object> GetContractInfoAsync(string contractAddress);
    }
}
