namespace Utilities.Interfaces
{
    public interface IHardhatExecutor : IContractUtilityExecutor
    {
        Task<string> CompileContract(string instancePath);
        Task<string> TestContract(string instancePath);
    }
}
