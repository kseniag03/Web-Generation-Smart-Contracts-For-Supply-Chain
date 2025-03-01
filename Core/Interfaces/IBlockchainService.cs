namespace Core.Interfaces
{
    public interface IBlockchainService
    {
        Task<string> DeployContractAsync(string contractBytecode);
        Task<string> SendTransactionAsync(string to, decimal amount);
    }
}
