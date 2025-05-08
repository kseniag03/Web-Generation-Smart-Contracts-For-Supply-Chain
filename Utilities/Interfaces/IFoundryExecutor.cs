namespace Utilities.Interfaces
{
    public interface IFoundryExecutor : IContractUtilityExecutor
    {
        Task<string> GetGasReport(string instancePath, string args = "");
    }
}
